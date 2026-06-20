using Avalime.UI.ViewModels;
using Avalime.ViewModels;
using Avalime.Core.Infra;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;

namespace Avalime.UI.Views.toolbar;

public class VmToolBar : ViewModelBase
	, IDisposable
{
	public VmIme Ime { get; }
	public RimeConnectionState RimeConnection { get; } = Di.GetRSvc<RimeConnectionState>();

	public str HanLabel => RimeConnection.IsSimplification ? "汉" : "漢";

	readonly PropertyChangedEventHandler _rimeConnectionPropertyChangedHandler;

	public VmToolBar(VmIme ime){
		Ime = ime;
		_rimeConnectionPropertyChangedHandler = (_, e) => {
			if(e.PropertyName == nameof(RimeConnectionState.IsSimplification)){
				OnPropertyChanged(nameof(HanLabel));
			}
		};
		RimeConnection.PropertyChanged += _rimeConnectionPropertyChangedHandler;
	}

	public void ToggleSimplification(){
		RimeConnection.ToggleSimplification();
	}

	public void ToggleClipboard(){
		Ime.ToggleClipboard();
	}

	public void ToggleRimeLog(){
		Ime.ToggleRimeLog();
	}

	public void Dispose()
	{
		RimeConnection.PropertyChanged -= _rimeConnectionPropertyChangedHandler;
	}
}
