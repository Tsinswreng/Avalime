using Avalime.ViewModels;

namespace Avalime.UI.views.input;
public class InputVm
	:ViewModelBase
{
	protected str _text = "";
	public str text{
		get{return _text;}
		set{SetProperty(ref _text, value);}
	}

}