using ViewImeControl = Avalime.UI.Views.Ime.ViewIme;
using Avalonia.Controls;
using Avalime.Core.Infra.Log;

namespace Avalime.UI.Views;

public class MainView : UserControl
	, IDisposable
{
	ViewImeControl? _viewIme;

	public MainView(){
		AppLog.Info($"[Life] MainView ctor #{GetHashCode()}");
		_viewIme = new ViewImeControl();
		Content = _viewIme;
	}

	public void Dispose()
	{
		AppLog.Info($"[Life] MainView dispose #{GetHashCode()}");
		_viewIme?.Dispose();
		_viewIme = null;
	}
}

