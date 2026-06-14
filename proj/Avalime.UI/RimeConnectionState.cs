using CommunityToolkit.Mvvm.ComponentModel;
using Avalime.Core.Keys;
using Avalime.Rime;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

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

	public void Connect(){
		LogInfo("Connect() begin");
		if(IsConnected){
			LogInfo("Connect() skipped: already connected");
			StatusText = "Rime 已連接";
			return;
		}
		StatusText = "正在連接 Rime";
		LogInfo("Creating RimeSetup");
		var setup = RimeSetup.Inst;
		LogInfo("RimeSetup created");
		var imeState = App.SvcP.GetRequiredService<ImeState>();
		LogInfo("Resolving ImeState done");
		imeState.ImeKeyProcessor = new RimeKeyProcessor(setup);
		LogInfo("ImeKeyProcessor switched to RimeKeyProcessor");
		Setup = setup;
		IsConnected = true;
		StatusText = "Rime 已連接";
		LogInfo("Connect() success");
	}

	public void SetError(str message){
		LogError(message);
		StatusText = message;
	}
}
