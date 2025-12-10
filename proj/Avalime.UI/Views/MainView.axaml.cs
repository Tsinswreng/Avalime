using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Avalime.UI.Views {
	public partial class MainView : UserControl {
		public MainView() {
			Content = new Avalime.UI.Views.KeyBoard.ViewKeyBoard();
		}
	}
}
