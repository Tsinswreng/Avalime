using Avalime.Core.Keys;
using Avalime.ViewModels;
using System.ComponentModel;

namespace Avalime.UI.Views.ToolBar;

public class VmToolBar : ViewModelBase
	, IDisposable
{
	public ImeUiState UiState { get; }
	public ISvcIme ImeState { get; }

	public str HanLabel => ImeState.IsSimplification ? "汉" : "漢";

	readonly PropertyChangedEventHandler _imePropertyChangedHandler;

	public VmToolBar(ImeUiState UiState, ISvcIme ImeState){
		this.UiState = UiState;
		this.ImeState = ImeState;
		_imePropertyChangedHandler = (_, e) => {
			if(e.PropertyName == nameof(ISvcIme.IsSimplification)){
				OnPropertyChanged(nameof(HanLabel));
			}
		};
		ImeState.PropertyChanged += _imePropertyChangedHandler;
	}

	public void ToggleSimplification(){
		_ = ImeState.ToggleSimplificationAsy();
	}

	public void ToggleClipboard(){
		UiState.ToggleClipboard();
	}

	public void ToggleRimeLog(){
		UiState.ToggleRimeLog();
	}

	public void Dispose()
	{
		ImeState.PropertyChanged -= _imePropertyChangedHandler;
	}
}
