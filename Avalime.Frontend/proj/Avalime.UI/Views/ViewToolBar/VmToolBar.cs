using Avalime.ViewModels;
using System.ComponentModel;

namespace Avalime.UI.Views.ViewToolBar;

public class VmToolBar : ViewModelBase
	, IDisposable
{
	public ImeUiState UiState { get; }
	public RimeConnectionState RimeConnection { get; }

	public str HanLabel => RimeConnection.IsSimplification ? "汉" : "漢";

	readonly PropertyChangedEventHandler _rimeConnectionPropertyChangedHandler;

	public VmToolBar(ImeUiState UiState, RimeConnectionState RimeConnection){
		this.UiState = UiState;
		this.RimeConnection = RimeConnection;
		_rimeConnectionPropertyChangedHandler = (_, e) => {
			if(e.PropertyName == nameof(RimeConnectionState.IsSimplification)){
				OnPropertyChanged(nameof(HanLabel));
			}
		};
		RimeConnection.PropertyChanged += _rimeConnectionPropertyChangedHandler;
	}

	public void ToggleSimplification(){
		RimeConnection.ToggleSimplification();
	}

	public void ToggleClipboard(){
		UiState.ToggleClipboard();
	}

	public void ToggleRimeLog(){
		UiState.ToggleRimeLog();
	}

	public void Dispose()
	{
		RimeConnection.PropertyChanged -= _rimeConnectionPropertyChangedHandler;
	}
}
