
using Avalonia.Controls;

namespace Avalime.UI.Views;

public partial class MainView : UserControl {
	public MainView() {
		//InitializeComponent();
		Content = new Avalime.UI.Views.KeyBoard.KeyBoard();

	}



}