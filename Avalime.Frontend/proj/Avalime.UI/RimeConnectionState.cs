using CommunityToolkit.Mvvm.ComponentModel;
using Avalime.Core.Keys;
using Avalime.Rime;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Threading;
using Avalonia.Threading;
using Rime.Api;

namespace Avalime.UI;

public partial class RimeConnectionState : ObservableObject
{
	static void LogInfo(str message){ Debug.WriteLine("[AvalimeRime] " + message); }
	static void LogError(str message){ Debug.WriteLine("[AvalimeRime] " + message); }

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

	public RimeConnectionState(){
		RimeSetup.OnOptionChanged += (name, enabled) => {
			if(name == "ascii_mode"){
				// 一律 Post 到 UI 線程：on_message 可能從 後台/P/Invoke 回調 觸發
				Dispatcher.UIThread.Post(() => {
					IsAsciiMode = enabled;
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

	public void Connect(){
		LogInfo("Connect() begin");
		if(IsConnected){
			LogInfo("Connect() skipped: already connected");
			StatusText = "Rime 已連接";
			return;
		}
		StatusText = "正在連接 Rime";
		LogInfo("Creating RimeSetup");
		try{
			var setup = RimeSetup.Inst;
			if(setup is null){
				SetError("Rime 初始化失敗：RimeSetup.Inst 為 null");
				return;
			}
			LogInfo("RimeSetup created");
			var imeState = App.SvcP.GetRequiredService<ImeState>();
			LogInfo("Resolving ImeState done");
			imeState.ImeKeyProcessor = new RimeKeyProcessor(setup);
			LogInfo("ImeKeyProcessor switched to RimeKeyProcessor");
			Setup = setup;
			IsConnected = true;
			StatusText = "Rime 已連接";
			LogInfo("Connect() success");
		}catch(Exception ex){
			LogError("Connect exception: " + ex);
			SetError("Rime 連接失敗: " + ex.Message);
		}
	}

	public void SetError(str message){
		LogError(message);
		StatusText = message;
	}
}
