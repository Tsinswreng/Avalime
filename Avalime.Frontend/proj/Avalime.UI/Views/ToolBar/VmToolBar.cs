using Avalime.Core.Keys;
using Avalime.ViewModels;
using Avalonia.Media;
using System.ComponentModel;

namespace Avalime.UI.Views.ToolBar;

public class VmToolBar : ViewModelBase
	, IDisposable
{
	public ImeUiState UiState { get; }
	public ISvcIme ImeState { get; }

	public str HanLabel => ImeState.IsSimplification ? "汉" : "漢";
	public IBrush CandidateCommentForeground => UiState.IsCandidateCommentVisible ? UiCfg.Inst.MainColor : Brushes.White;
	public IBrush SplitKeyboardForeground => UiState.IsSplitKeyboardEnabled ? UiCfg.Inst.MainColor : Brushes.White;

	readonly PropertyChangedEventHandler _imePropertyChangedHandler;
	readonly PropertyChangedEventHandler _uiStatePropertyChangedHandler;

	public VmToolBar(ImeUiState UiState, ISvcIme ImeState){
		this.UiState = UiState;
		this.ImeState = ImeState;
		_imePropertyChangedHandler = (_, e) => {
			if(e.PropertyName == nameof(ISvcIme.IsSimplification)){
				OnPropertyChanged(nameof(HanLabel));
			}
		};
		_uiStatePropertyChangedHandler = (_, e) => {
			if(e.PropertyName == nameof(ImeUiState.IsCandidateCommentVisible)){
				OnPropertyChanged(nameof(CandidateCommentForeground));
			}
			if(e.PropertyName == nameof(ImeUiState.IsSplitKeyboardEnabled)){
				OnPropertyChanged(nameof(SplitKeyboardForeground));
			}
		};
		ImeState.PropertyChanged += _imePropertyChangedHandler;
		UiState.PropertyChanged += _uiStatePropertyChangedHandler;
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

	public void ToggleCandidateComment(){
		UiState.ToggleCandidateComment();
	}

	public void ToggleSplitKeyboard(){
		UiState.ToggleSplitKeyboard();
	}

	public void Dispose()
	{
		ImeState.PropertyChanged -= _imePropertyChangedHandler;
		UiState.PropertyChanged -= _uiStatePropertyChangedHandler;
	}
}
