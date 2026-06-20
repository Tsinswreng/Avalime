using Avalime.UI.Views.ViewIme;
using Avalonia.Controls;

namespace Avalime.UI.Views.ViewMainView;

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
