//主視圖：根節點，掛載鍵盤
using Avalonia.Controls;
using Avalime.UI.Views.KeyBoard;

namespace Avalime.UI.Views;

public class MainView : UserControl
{
	public MainView(){
		Content = new ViewKeyBoard();
	}
}
