using ViewImeControl = Avalime.UI.Views.ViewIme.ViewIme;
using Avalonia.Controls;

namespace Avalime.UI.Views;

public class MainView : UserControl
	, IDisposable
{
	ViewImeControl? _viewIme;

	public MainView(){
		_viewIme = new ViewImeControl();
		Content = _viewIme;
	}

	public void Dispose()
	{
		_viewIme?.Dispose();
		_viewIme = null;
	}
}
