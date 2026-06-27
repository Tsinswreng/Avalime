using Avalime.Core.Keys;
using Avalime.ViewModels;
using Avalonia.Threading;
using System.ComponentModel;

namespace Avalime.UI.Views.Ime;

public class VmIme : ViewModelBase
	, IDisposable
{
	public ISvcIme ImeState { get; }
	public ImeUiState UiState { get; }

	public bool IsComposing{
		get => field;
		set => SetProperty(ref field, value);
	}

	public bool IsClipboardVisible => UiState.IsClipboardVisible;

	public bool IsRimeLogVisible => UiState.IsRimeLogVisible;

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
	readonly PropertyChangedEventHandler _uiStatePropertyChangedHandler;
	readonly PropertyChangedEventHandler _imePropertyChangedHandler;

	public VmIme(ISvcIme ImeState, ImeUiState UiState){
		this.ImeState = ImeState;
		this.UiState = UiState;
		_propertyChangedHandler = (_, e) => {
			if(e.PropertyName is nameof(IsComposing) or nameof(HasPreedit)){
				OnPropertyChanged(nameof(ShowToolbar));
				OnPropertyChanged(nameof(ShowCandidates));
				OnPropertyChanged(nameof(ShowPreedit));
				OnPropertyChanged(nameof(ShowKeyboard));
				OnPropertyChanged(nameof(ShowClipboard));
				OnPropertyChanged(nameof(ShowRimeLog));
			}
		};
		PropertyChanged += _propertyChangedHandler;
		_uiStatePropertyChangedHandler = (_, e) => {
			if(e.PropertyName is nameof(ImeUiState.IsClipboardVisible) or nameof(ImeUiState.IsRimeLogVisible)){
				OnPropertyChanged(nameof(IsClipboardVisible));
				OnPropertyChanged(nameof(IsRimeLogVisible));
				OnPropertyChanged(nameof(ShowToolbar));
				OnPropertyChanged(nameof(ShowCandidates));
				OnPropertyChanged(nameof(ShowPreedit));
				OnPropertyChanged(nameof(ShowKeyboard));
				OnPropertyChanged(nameof(ShowClipboard));
				OnPropertyChanged(nameof(ShowRimeLog));
			}
		};
		UiState.PropertyChanged += _uiStatePropertyChangedHandler;

		_afterInputHandler = (_, _) => RefreshCompositionState();
		ImeState.AfterInput += _afterInputHandler;
		_imePropertyChangedHandler = OnImePropertyChanged;
		ImeState.PropertyChanged += _imePropertyChangedHandler;
		SyncRimeLogVisibility();
		_ = ImeState.ConnectAsy();
	}

	void OnImePropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if(e.PropertyName is nameof(ISvcIme.IsConnecting) or nameof(ISvcIme.IsConnected)){
			SyncRimeLogVisibility();
		}
	}

	void SyncRimeLogVisibility()
	{
		UiState.SetForcedRimeLogVisible(ImeState.IsConnecting || !ImeState.IsConnected);
	}

	void RefreshCompositionState(){
		var composing = ImeState.IsComposing;
		var hasPreedit = !string.IsNullOrEmpty(ImeState.Preedit);
		Dispatcher.UIThread.Post(() => {
			IsComposing = composing;
			HasPreedit = hasPreedit;
		});
	}

	public void Dispose()
	{
		PropertyChanged -= _propertyChangedHandler;
		UiState.PropertyChanged -= _uiStatePropertyChangedHandler;
		ImeState.AfterInput -= _afterInputHandler;
		ImeState.PropertyChanged -= _imePropertyChangedHandler;
	}
}
