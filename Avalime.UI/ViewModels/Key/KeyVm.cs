using CommunityToolkit.Mvvm.ComponentModel;

namespace Avalime.ViewModels.Key;

public partial class KeyVm
	:ViewModelBase
{

	public KeyVm(){}

	protected str _label="";
	public str label{
		get => _label;
		set => SetProperty(ref _label, value);
	}

}
