using Avalonia.Controls;

namespace Avalime.UI.Views;

public class MainView : UserControl
	, IDisposable
{
	ViewIme? _viewIme;

	public MainView(){
		_viewIme = new ViewIme();
		Content = _viewIme;
	}

	public void Dispose()
	{
		_viewIme?.Dispose();
		_viewIme = null;
	}
}
