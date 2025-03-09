
using Avalonia.Controls;

namespace Avalime.UI.views;

public partial class MainView : UserControl {
	public MainView() {
		//InitializeComponent();
		Content = new Avalime.UI.views.KeyBoard.KeyBoard();

	}



}