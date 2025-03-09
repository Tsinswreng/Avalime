using Avalonia;
using Avalonia.Controls;

namespace Avalime.UI.views;

public partial class MainWindow : Window {
	public MainWindow() {
		//InitializeComponent();
		Content=new MainView();
		this.AttachDevTools();
	}
}