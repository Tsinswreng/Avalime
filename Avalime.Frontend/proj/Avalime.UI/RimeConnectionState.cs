using CommunityToolkit.Mvvm.ComponentModel;
using Avalime.Core.Infra.Log;
using Avalime.Core.Keys;
using Avalime.Rime;
using System.Threading;
using Avalonia.Threading;
using Rime.Api;
using Microsoft.Extensions.Logging;

namespace Avalime.UI;

public partial class RimeConnectionState : ObservableObject
{
	readonly SvcState _imeState;
	static void LogInfo(str message){
		AppLog.Info("[AvalimeRime] " + message);
	}
	static void LogError(str message){
		AppLog.Error("[AvalimeRime] " + message);
	}

	public bool IsConnected{
		get => field;
		private set => SetProperty(ref field, value);
	}

	public str StatusText{
		get => field;
		private set => SetProperty(ref field, value);
	} = "Rime 未連接";

	public RimeSetup? Setup{
		get => field;
		private set => SetProperty(ref field, value);
	}

	public bool IsAsciiMode{
		get => field;
		private set => SetProperty(ref field, value);
	}

	public bool IsSimplification{
		get => field;
		private set => SetProperty(ref field, value);
	}

	public bool IsConnecting{
		get => field;
		private set => SetProperty(ref field, value);
	}

	public RimeConnectionState(SvcState ImeState){
		_imeState = ImeState;
		RimeSetup.OnOptionChanged += (name, enabled) => {
			if(name == "ascii_mode"){
				// 一律 Post 到 UI 線程：on_message 可能從 後台/P/Invoke 回調 觸發
				Dispatcher.UIThread.Post(() => {
					IsAsciiMode = enabled;
				});
			}
			if(name == "simplification"){
				Dispatcher.UIThread.Post(() => {
					IsSimplification = enabled;
				});
			}
		};
	}

	/// <summary>防止 set_option (~350ms) 阻塞 UI 線程導致 ANR，以及逆向 P/Invoke 中觸發 Mono !ji->async 崩潰</summary>
	int _toggleBusy = 0;

	unsafe public void ToggleAsciiMode(){
		var rime = Setup;
		if(rime is null) return;

		// 防連點：如果上一次 toggle 還沒完成就跳過
		if(Interlocked.CompareExchange(ref _toggleBusy, 1, 0) != 0) return;

		Task.Run(() => {
			try{
				ReadOnlySpan<byte> opt = "ascii_mode\0"u8;
				fixed(byte* p = opt){
					var current = rime.apiFn.get_option(rime.rimeSessionId, p);
					var newValue = current == 0 ? RimeUtil.True : RimeUtil.False;
					rime.apiFn.set_option(rime.rimeSessionId, p, newValue);
					var isAscii = newValue != RimeUtil.False;

					// 直接在 UI 線程更新屬性（不依賴 on_message 回調）
					Dispatcher.UIThread.Post(() => {
						IsAsciiMode = isAscii;
					});
				}
			}catch(Exception ex){
				LogError("ToggleAsciiMode error: " + ex);
			}finally{
				Interlocked.Exchange(ref _toggleBusy, 0);
			}
		});
	}

	public async Task<nil> ConnectAsy(CT ct = default){
		LogInfo("Connect() begin");
		if(IsConnected){
			LogInfo("Connect() skipped: already connected");
			await Dispatcher.UIThread.InvokeAsync(() => {
				StatusText = "Rime 已連接";
			});
			return NIL;
		}
		try{
			await Dispatcher.UIThread.InvokeAsync(() => {
				IsConnecting = true;
				StatusText = "正在連接 Rime";
			});
			LogInfo("Creating RimeSetup");
			ct.ThrowIfCancellationRequested();
			var setup = await Task.Run(() => RimeSetup.Inst, ct);
			if(setup is null){
				SetError("Rime 初始化失敗：RimeSetup.Inst 為 null");
				return NIL;
			}
			LogInfo("RimeSetup created");
			await Dispatcher.UIThread.InvokeAsync(() => {
				LogInfo("Resolving ImeState done");
				_imeState.ImeKeyProcessor = new RimeKeyProcessor(setup);
				LogInfo("ImeKeyProcessor switched to RimeKeyProcessor");
				Setup = setup;
				IsConnected = true;
				StatusText = "Rime 已連接";
			});
			RefreshOptions();
			LogInfo("Connect() success");
		}catch(OperationCanceledException){
			await Dispatcher.UIThread.InvokeAsync(() => {
				StatusText = IsConnected ? "Rime 已連接" : "已取消連接 Rime";
			});
		}catch(Exception ex){
			LogError("Connect exception: " + ex);
			SetError("Rime 連接失敗: " + ex.Message);
		}finally{
			await Dispatcher.UIThread.InvokeAsync(() => {
				IsConnecting = false;
			});
		}
		return NIL;
	}

	unsafe void RefreshOptions(){
		var rime = Setup;
		if(rime is null){
			return;
		}
		ReadOnlySpan<byte> asciiMode = "ascii_mode\0"u8;
		ReadOnlySpan<byte> simplification = "simplification\0"u8;
		fixed(byte* pAscii = asciiMode)
		fixed(byte* pSimplification = simplification){
			var isAscii = rime.apiFn.get_option(rime.rimeSessionId, pAscii) != RimeUtil.False;
			var isSimplified = rime.apiFn.get_option(rime.rimeSessionId, pSimplification) != RimeUtil.False;
			Dispatcher.UIThread.Post(() => {
				IsAsciiMode = isAscii;
				IsSimplification = isSimplified;
			});
		}
	}

	int _toggleSimplificationBusy = 0;

	unsafe public void ToggleSimplification(){
		var rime = Setup;
		if(rime is null) return;
		if(Interlocked.CompareExchange(ref _toggleSimplificationBusy, 1, 0) != 0) return;

		Task.Run(() => {
			try{
				ReadOnlySpan<byte> opt = "simplification\0"u8;
				fixed(byte* p = opt){
					var current = rime.apiFn.get_option(rime.rimeSessionId, p);
					var newValue = current == 0 ? RimeUtil.True : RimeUtil.False;
					rime.apiFn.set_option(rime.rimeSessionId, p, newValue);
					var enabled = newValue != RimeUtil.False;
					Dispatcher.UIThread.Post(() => {
						IsSimplification = enabled;
					});
				}
			}catch(Exception ex){
				LogError("ToggleSimplification error: " + ex);
			}finally{
				Interlocked.Exchange(ref _toggleSimplificationBusy, 0);
			}
		});
	}

	public void SetError(str message){
		LogError(message);
		StatusText = message;
	}
}
