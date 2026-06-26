using Avalime.Core.Ime;
using Avalime.Core.Infra.Log;
using Avalime.Core.Keys;
using Rime.Api;
using Rime.Api.Types;
using Tsinswreng.CsInterop;

namespace Avalime.Rime;

/// <summary>
/// ISvcIme 的 Rime 引擎實現。
/// 負責把 Rime 引擎狀態回填到 Core 狀態模型。
/// </summary>
unsafe public class SvcIme : ISvcIme
{
	readonly RimeSetup _RimeSetup;
	readonly Lock _StateLock = new();
	i32 _IsTogglingAscii;
	i32 _IsTogglingSimplification;

	public SvcIme(
		IOsKeyProcessor OsKeyProcessor
		, IImeKeyProcessor ImeKeyProcessor
		, RimeSetup RimeSetup
	) : base(OsKeyProcessor, ImeKeyProcessor){
		_RimeSetup = RimeSetup;
		AfterInput += OnAfterInput;
		RimeSetup.OnOptionChanged += OnOptionChanged;
	}

	/// <summary>
	/// 真正建立 Core 與 Rime session 的連通狀態。
	/// </summary>
	public override Task ConnectAsy(CT Ct = default){
		Ct.ThrowIfCancellationRequested();
		IsConnecting = true;
		StatusText = "正在連接 Rime";
		try{
			_ = _RimeSetup.rimeSessionId;
			SyncStateFromRime();
			IsConnected = true;
			StatusText = "Rime 已連接";
		}catch(Exception Ex){
			IsConnected = false;
			StatusText = "Rime 連接失敗: " + Ex.Message;
			throw;
		}finally{
			IsConnecting = false;
		}
		return Task.CompletedTask;
	}

	/// <summary>
	/// 通過 Rime API 切換 ascii_mode。
	/// </summary>
	public override Task ToggleAsciiModeAsy(CT Ct = default){
		if(Interlocked.Exchange(ref _IsTogglingAscii, 1) != 0){
			return Task.CompletedTask;
		}
		return Task.Run(() => {
			try{
				Ct.ThrowIfCancellationRequested();
				lock(_StateLock){
					var Rime = _RimeSetup.apiFn;
					var SessionId = _RimeSetup.rimeSessionId;
					byte* Option = stackalloc byte[15];
					WriteAsciiMode(new Span<byte>(Option, 15));
					var Current = Rime.get_option(SessionId, Option);
					Rime.set_option(SessionId, Option, Current == RimeUtil.False ? RimeUtil.True : RimeUtil.False);
				}
				SyncStateFromRime();
			}finally{
				Interlocked.Exchange(ref _IsTogglingAscii, 0);
			}
		}, Ct);
	}

	/// <summary>
	/// 通過 Rime API 切換 simplification。
	/// </summary>
	public override Task ToggleSimplificationAsy(CT Ct = default){
		if(Interlocked.Exchange(ref _IsTogglingSimplification, 1) != 0){
			return Task.CompletedTask;
		}
		return Task.Run(() => {
			try{
				Ct.ThrowIfCancellationRequested();
				lock(_StateLock){
					var Rime = _RimeSetup.apiFn;
					var SessionId = _RimeSetup.rimeSessionId;
					byte* Option = stackalloc byte[15];
					WriteSimplification(new Span<byte>(Option, 15));
					var Current = Rime.get_option(SessionId, Option);
					Rime.set_option(SessionId, Option, Current == RimeUtil.False ? RimeUtil.True : RimeUtil.False);
				}
				SyncStateFromRime();
			}finally{
				Interlocked.Exchange(ref _IsTogglingSimplification, 0);
			}
		}, Ct);
	}

	/// <summary>
	/// 每次輸入後從 Rime 抓最新快照，回流到 Core 狀態。
	/// </summary>
	void OnAfterInput(object? Sender, IEnumerable<IKeyEvent> Args){
		try{
			SyncStateFromRime();
		}catch(Exception Ex){
			AppLog.Error(Ex, "[AvalimeRime] SyncStateFromRime after input failed");
		}
	}

	/// <summary>
	/// 逆向 P/Invoke 回調只傳 option 名和值，實際狀態同步仍統一回到這裡。
	/// </summary>
	void OnOptionChanged(str OptionName, bool IsEnabled){
		try{
			switch(OptionName){
				case "ascii_mode":
					IsAsciiMode = IsEnabled;
					break;
				case "simplification":
					IsSimplification = IsEnabled;
					break;
			}
		}catch(Exception Ex){
			AppLog.Error(Ex, "[AvalimeRime] OnOptionChanged sync failed");
		}
	}

	/// <summary>
	/// 將 Rime session 的 status/context 轉成 Core 狀態。
	/// </summary>
	void SyncStateFromRime(){
		lock(_StateLock){
			var Rime = _RimeSetup.apiFn;
			var SessionId = _RimeSetup.rimeSessionId;
			if(SessionId == 0){
				IsConnected = false;
				StatusText = "Rime session 無效";
				ReplacePreedit("");
				ReplaceCandidates(new());
				IsComposing = false;
				return;
			}

			var Status = new RimeStatus{
				data_size = RimeUtil.DataSize<RimeStatus>()
			};
			if(Rime.get_status(SessionId, &Status) != RimeUtil.False){
				try{
					IsAsciiMode = Status.is_ascii_mode != RimeUtil.False;
					IsSimplification = Status.is_simplified != RimeUtil.False;
					IsComposing = Status.is_composing != RimeUtil.False;
				}finally{
					Rime.free_status(&Status);
				}
			}

			var Context = new RimeContext{
				data_size = RimeUtil.DataSize<RimeContext>()
			};
			if(Rime.get_context(SessionId, &Context) != RimeUtil.False){
				try{
					ReplacePreedit(MkDisplayedPreedit(Context.composition));
					ReplaceCandidates(ReadCandidates(&Context));
				}finally{
					Rime.free_context(&Context);
				}
			}else{
				ReplacePreedit("");
				ReplaceCandidates(new());
			}
		}
	}

	/// <summary>
	/// 先按 librime 的 soft cursor 思路，把 composition.preedit 與 cursor_pos 組裝成當前要顯示的 preedit。
	/// 目前先插入 U+2038 `‸`，優先補齊 Avalime 缺失的輸入游標顯示。
	/// </summary>
	str MkDisplayedPreedit(RimeComposition composition) {
		var preedit = ToolCStr.ToCsStr(composition.preedit) ?? "";
		if(preedit.Length <= 0){
			return preedit;
		}

		const str Caret = "\u2038";
		var cursorPos = composition.cursor_pos;
		if(cursorPos < 0){
			cursorPos = 0;
		}
		if(cursorPos > preedit.Length){
			cursorPos = preedit.Length;
		}
		return preedit.Insert(cursorPos, Caret);
	}

	/// <summary>
	/// 在 native context 有效期間把候選詞複製成 Core DTO，包含分頁訊息。
	/// </summary>
	CandidatePage ReadCandidates(RimeContext* Context){
		var Ans = new List<ICandidate>();
		var Menu = Context->menu;
		for(i32 Index = 0; Index < Menu.num_candidates; Index++){
			var Candidate = Menu.candidates[Index];
			Ans.Add(new Candidate{
				Text = ToolCStr.ToCsStr(Candidate.text) ?? ""
				, Comment = ToolCStr.ToCsStr(Candidate.comment) ?? ""
			});
		}
		return new CandidatePage{
			Data = Ans,
			PageIdx = (u64)Menu.page_no,
			PageSize = (u64)Menu.page_size,
			IsLastPage = Menu.is_last_page != RimeUtil.False,
			HighlightedIndex = Menu.highlighted_candidate_index,
		};
	}

	/// <summary>
	/// 把 ascii_mode 寫入 stackalloc buffer。
	/// </summary>
	static void WriteAsciiMode(Span<byte> Buffer){
		Buffer.Clear();
		"ascii_mode"u8.CopyTo(Buffer);
	}

	/// <summary>
	/// 把 simplification 寫入 stackalloc buffer。
	/// </summary>
	static void WriteSimplification(Span<byte> Buffer){
		Buffer.Clear();
		"simplification"u8.CopyTo(Buffer);
	}
}
