using Avalime.Core.Infra.Log;
using Avalime.Core.Keys;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Avalime.UI;

public partial class RimeConnectionState : ObservableObject
{
	readonly ISvcIme _imeState;
	readonly EventHandler _connectionStateChangedHandler;
	readonly EventHandler _stateChangedHandler;

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
	} = "Ime 未連接";

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

	public RimeConnectionState(ISvcIme ImeState){
		_imeState = ImeState;
		_connectionStateChangedHandler = (_, _) => SyncFromImeState();
		_stateChangedHandler = (_, _) => SyncFromImeState();
		_imeState.ConnectionStateChanged += _connectionStateChangedHandler;
		_imeState.StateChanged += _stateChangedHandler;
		SyncFromImeState();
	}

	void SyncFromImeState(){
		Dispatcher.UIThread.Post(() => {
			IsConnected = _imeState.IsConnected;
			IsConnecting = _imeState.IsConnecting;
			IsAsciiMode = _imeState.IsAsciiMode;
			IsSimplification = _imeState.IsSimplification;
			StatusText = _imeState.StatusText;
		});
	}

	public void ToggleAsciiMode(){
		_ = _imeState.ToggleAsciiModeAsy();
	}

	public async Task<nil> ConnectAsy(CT ct = default){
		LogInfo("Connect() begin");
		try{
			await _imeState.ConnectAsy(ct);
			SyncFromImeState();
			LogInfo("Connect() success");
		}catch(OperationCanceledException){
			SyncFromImeState();
		}catch(Exception ex){
			LogError("Connect exception: " + ex);
			SetError("Ime 連接失敗: " + ex.Message);
		}
		return NIL;
	}

	public void ToggleSimplification(){
		_ = _imeState.ToggleSimplificationAsy();
	}

	public void SetError(str message){
		LogError(message);
		StatusText = message;
	}
}
