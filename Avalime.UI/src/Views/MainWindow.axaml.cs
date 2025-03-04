using Avalonia.Controls;

namespace Avalime.UI.Views;

public partial class MainWindow : Window {
	public MainWindow() {
		//InitializeComponent();
		Content=new MainView();
	}
}