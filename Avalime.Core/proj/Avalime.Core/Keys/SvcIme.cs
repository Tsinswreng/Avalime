namespace Avalime.Core.Keys;
using Avalime.Core.Infra.Log;
using Avalime.Core.Ime;



[Doc(@$"輸入法引擎抽象與適配。
外部訪問其API當由{nameof(ISvcIme)}引用而非{nameof(SvcIme)}。
暫時定義爲class 後續會改成interface。
")]
public class ISvcIme
	//:I_ImeState //TODO 先叶後抽象
{
	public IDictionary<object, object?> Cfg{get;set;}= new Dictionary<object, object?>();
	public IOsKeyProcessor? OsKeyProcessor{get;set;}
	public IImeKeyProcessor? ImeKeyProcessor{get;set;}

	public bool IsConnected{
		get => field;
		set{
			if(field == value){return;}
			field = value;
			ConnectionStateChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	public bool IsConnecting{
		get => field;
		set{
			if(field == value){return;}
			field = value;
			ConnectionStateChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	public bool IsAsciiMode{
		get => field;
		set{
			if(field == value){return;}
			field = value;
			StateChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	public bool IsSimplification{
		get => field;
		set{
			if(field == value){return;}
			field = value;
			StateChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	public bool IsComposing{
		get => field;
		set{
			if(field == value){return;}
			field = value;
			StateChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	public str StatusText{
		get => field;
		set{
			if(field == value){return;}
			field = value;
			ConnectionStateChanged?.Invoke(this, EventArgs.Empty);
		}
	} = "Ime 未連接";

	public str Preedit{
		get => field;
		set{
			if(field == value){return;}
			field = value;
			StateChanged?.Invoke(this, EventArgs.Empty);
		}
	} = "";

	public IReadOnlyList<ICandidate> Candidates{
		get => field;
		set{
			field = value;
			StateChanged?.Invoke(this, EventArgs.Empty);
		}
	} = [];

	public ISvcIme(IOsKeyProcessor osKeyProcessor, IImeKeyProcessor imeKeyProcessor) {
		this.OsKeyProcessor = osKeyProcessor;
		this.ImeKeyProcessor = imeKeyProcessor;
	}

	public ISvcIme(IOsKeyProcessor osKeyProcessor) {
		this.OsKeyProcessor = osKeyProcessor;
	}

	public ISvcIme() {
	}

	public void InputSafely(IEnumerable<IKeyEvent> keyEvents, Action<Exception>? onError = null){
		_ = Task.Run(async () => {
			try{
				await Input(keyEvents, default);
			}catch(Exception ex){
				AppLog.Error(ex, "[ImeState] InputSafely error");
				onError?.Invoke(ex);
			}
		});
	}

	public async Task<RespInput> Input(IEnumerable<IKeyEvent> keyEvents, CT Ct){
		var sw = System.Diagnostics.Stopwatch.StartNew();
		AppLog.Debug($"[Perf] ImeState.Input start: {sw.ElapsedMilliseconds}ms");
		BeforeInput?.Invoke(this, keyEvents);
		IRespOnKeyEvent? resp = null;
		if(ImeKeyProcessor is not null){
			var swProc = System.Diagnostics.Stopwatch.StartNew();
			resp = await ImeKeyProcessor.OnKeyEvents(keyEvents, Ct);
			AppLog.Debug($"[Perf] ImeState.Input OnKeyEventsAsy done: {swProc.ElapsedMilliseconds}ms");
		}
		var swAfter = System.Diagnostics.Stopwatch.StartNew();
		AfterInput?.Invoke(this, keyEvents);
		AppLog.Debug($"[Perf] ImeState.Input AfterInput done: {swAfter.ElapsedMilliseconds}ms");

		// 觸發 commit 事件
		if(resp?.Commits is not null){
			foreach(var commitText in resp.Commits){
				OnCommit?.Invoke(this, commitText);
			}
		}

		// 未處理按鍵轉發給 OS
		if(resp?.UnhandledKeys is not null && resp.UnhandledKeys.Count > 0 && OsKeyProcessor is not null){
			var swOs = System.Diagnostics.Stopwatch.StartNew();
			await OsKeyProcessor.OnKeyEvents(resp.UnhandledKeys, Ct);
			AppLog.Debug($"[Perf] ImeState.Input OsKeyProcessor done: {swOs.ElapsedMilliseconds}ms, unhandled: {resp.UnhandledKeys.Count}");
		}

		AppLog.Debug($"[Perf] ImeState.Input total done: {sw.ElapsedMilliseconds}ms");
		return new();
	}

	public virtual Task ConnectAsy(CT ct = default){
		IsConnected = ImeKeyProcessor is not null;
		if(IsConnected && string.IsNullOrWhiteSpace(StatusText)){
			StatusText = "Ime 已連接";
		}
		return Task.CompletedTask;
	}

	public virtual Task ToggleAsciiModeAsy(CT ct = default){
		IsAsciiMode = !IsAsciiMode;
		return Task.CompletedTask;
	}

	public virtual Task ToggleSimplificationAsy(CT ct = default){
		IsSimplification = !IsSimplification;
		return Task.CompletedTask;
	}

	public void ReplaceCandidates(IEnumerable<ICandidate>? candidates){
		Candidates = candidates?.ToArray() ?? [];
	}

	public void ReplacePreedit(str? preedit){
		Preedit = preedit ?? "";
	}

	public event EventHandler<IEnumerable<IKeyEvent>>? BeforeInput;
	public event EventHandler<IEnumerable<IKeyEvent>>? AfterInput;
	public event EventHandler? StateChanged;
	public event EventHandler? ConnectionStateChanged;

	/// 當 Rime 引擎 commit 文字時觸發。參數為 commit 的文字內容。
	public event EventHandler<string>? OnCommit;

}


[Doc(@$"實現類")]
public class SvcIme:ISvcIme{

}
