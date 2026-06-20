using Avalime.Core.Keys;
using Avalime.UI;
using Avalime.ViewModels;

namespace Avalime.UI.Views.ViewKeyBoard;
using Ctx = VmKeyBoard;

public partial class VmKeyBoard : ViewModelBase
{
	public ImeState ImeState{get;set;}

	public VmKeyBoard(ImeState ImeState){
		this.ImeState = ImeState;
	}

	public bool IsNumLayout{
		get => field;
		set => SetProperty(ref field, value);
	}

	public bool IsShiftLocked{
		get => field;
		set => SetProperty(ref field, value);
	}
}
