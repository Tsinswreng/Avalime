using Avalime.UI.ViewModels;
using Avalime.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Avalime.UI.Views.toolbar;

public class VmToolBar : ViewModelBase
{
	public VmIme Ime { get; }
	public RimeConnectionState RimeConnection { get; } = App.SvcP.GetRequiredService<RimeConnectionState>();

	public str HanLabel => RimeConnection.IsSimplification ? "汉" : "漢";

	public VmToolBar(VmIme ime){
		Ime = ime;
		RimeConnection.PropertyChanged += (_, e) => {
			if(e.PropertyName == nameof(RimeConnectionState.IsSimplification)){
				OnPropertyChanged(nameof(HanLabel));
			}
		};
	}

	public void ToggleSimplification(){
		RimeConnection.ToggleSimplification();
	}

	public void ToggleClipboard(){
		Ime.ToggleClipboard();
	}
}
