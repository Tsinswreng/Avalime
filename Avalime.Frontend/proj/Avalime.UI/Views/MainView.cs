using Avalonia.Controls;
using Avalime.UI.Views.KeyBoard;

namespace Avalime.UI.Views;

public class MainView : UserControl
{
	public MainView(){
		Content = new ViewKeyBoard();
	}
}
