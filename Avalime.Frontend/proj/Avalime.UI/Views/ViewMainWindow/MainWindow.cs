using Avalime.UI.Views.ViewMainView;
using Avalonia.Controls;

namespace Avalime.UI.Views.ViewMainWindow;

public class MainWindow : Window
{
	public MainWindow(){
		Content = new MainView();
	}
}
