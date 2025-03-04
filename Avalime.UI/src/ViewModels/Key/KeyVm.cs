using Avalonia.Input;
using Avalonia.Remote.Protocol.Input;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Avalime.ViewModels.key;

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
