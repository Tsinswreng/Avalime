using Avalime.Core.Keys;
using Avalime.Rime;
using Avalime.ViewModels;
using Avalonia.Threading;
using Rime.Api;
using System.ComponentModel;
using Tsinswreng.CsInterop;

namespace Avalime.UI.ViewModels;

public class VmIme : ViewModelBase
	, IDisposable
{
	public ImeState ImeState { get; }
	public RimeConnectionState RimeConnection { get; }

	public bool IsComposing{
		get => field;
		set => SetProperty(ref field, value);
	}

	public bool IsClipboardVisible{
		get => field;
		set => SetProperty(ref field, value);
	}

	public bool IsRimeLogVisible{
		get => field;
		set => SetProperty(ref field, value);
	}

	public bool HasPreedit{
		get => field;
		set => SetProperty(ref field, value);
	}

	public bool ShowToolbar => !IsComposing || IsClipboardVisible;
	public bool ShowCandidates => (IsComposing || HasPreedit) && !IsClipboardVisible;
	public bool ShowPreedit => !IsClipboardVisible;
	public bool ShowKeyboard => !IsClipboardVisible && !IsRimeLogVisible;
	public bool ShowClipboard => IsClipboardVisible;
	public bool ShowRimeLog => IsRimeLogVisible;

	readonly PropertyChangedEventHandler _propertyChangedHandler;
	readonly EventHandler<IEnumerable<IKeyEvent>> _afterInputHandler;

	public VmIme(ImeState ImeState, RimeConnectionState RimeConnection){
		this.ImeState = ImeState;
		this.RimeConnection = RimeConnection;
		_propertyChangedHandler = (_, e) => {
			if(
				e.PropertyName is nameof(IsComposing)
				or nameof(HasPreedit)
				or nameof(IsClipboardVisible)
				or nameof(IsRimeLogVisible)
			){
				OnPropertyChanged(nameof(ShowToolbar));
				OnPropertyChanged(nameof(ShowCandidates));
				OnPropertyChanged(nameof(ShowPreedit));
				OnPropertyChanged(nameof(ShowKeyboard));
				OnPropertyChanged(nameof(ShowClipboard));
				OnPropertyChanged(nameof(ShowRimeLog));
			}
		};
		PropertyChanged += _propertyChangedHandler;

		_afterInputHandler = (_, _) => RefreshCompositionState();
		ImeState.AfterInput += _afterInputHandler;
		RimeConnection.PropertyChanged += OnRimeConnectionPropertyChanged;
		SyncRimeLogVisibility();
		_ = RimeConnection.ConnectAsy();
	}

	void OnRimeConnectionPropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if(e.PropertyName is nameof(RimeConnectionState.IsConnecting) or nameof(RimeConnectionState.IsConnected)){
			SyncRimeLogVisibility();
		}
	}

	void SyncRimeLogVisibility()
	{
		if(RimeConnection.IsConnecting || !RimeConnection.IsConnected){
			IsRimeLogVisible = true;
		}
	}

	unsafe void RefreshCompositionState(){
		var rime = RimeConnection.Setup;
		if(rime is null){
			Dispatcher.UIThread.Post(() => {
				IsComposing = false;
				HasPreedit = false;
			});
			return;
		}
		var status = new RimeStatus{
			data_size = RimeUtil.DataSize<RimeStatus>()
		};
		var gotStatus = rime.apiFn.get_status(rime.rimeSessionId, &status) == RimeUtil.True;
		var composing = gotStatus && status.is_composing != RimeUtil.False;
		if(gotStatus){
			rime.apiFn.free_status(&status);
		}

		var ctx = new RimeContext{
			data_size = RimeUtil.DataSize<RimeContext>()
		};
		var gotContext = rime.apiFn.get_context(rime.rimeSessionId, &ctx) == RimeUtil.True;
		var hasPreedit = false;
		if(gotContext){
			hasPreedit = ctx.composition.length > 0 || !string.IsNullOrEmpty(ToolCStr.ToCsStr(ctx.composition.preedit));
			rime.apiFn.free_context(&ctx);
		}

		Dispatcher.UIThread.Post(() => {
			IsComposing = composing;
			HasPreedit = hasPreedit;
		});
	}

	public void ToggleClipboard(){
		if(!IsClipboardVisible){
			IsRimeLogVisible = false;
		}
		IsClipboardVisible = !IsClipboardVisible;
	}

	public void ExitClipboard(){
		IsClipboardVisible = false;
	}

	public void ToggleRimeLog()
	{
		if(!IsRimeLogVisible){
			IsClipboardVisible = false;
		}
		IsRimeLogVisible = !IsRimeLogVisible;
	}

	public void ExitRimeLog()
	{
		IsRimeLogVisible = false;
	}

	public void Dispose()
	{
		PropertyChanged -= _propertyChangedHandler;
		ImeState.AfterInput -= _afterInputHandler;
		RimeConnection.PropertyChanged -= OnRimeConnectionPropertyChanged;
	}
}
