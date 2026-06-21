using Avalime.Core.Keys;
using Avalime.UI;
using Avalime.ViewModels;

namespace Avalime.UI.Views.KeyBoard;
using Ctx = VmKeyBoard;

public partial class VmKeyBoard : ViewModelBase
{
	public ISvcIme ImeState{get;set;}

	public VmKeyBoard(ISvcIme ImeState){
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

