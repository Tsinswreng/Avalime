using System.Collections.ObjectModel;
using System.ComponentModel;
using Avalime.Core.Infra;
using Avalime.UI.ViewModels;
using Avalime.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Avalime.UI.Views.clipboard;

public class VmClipboard : ViewModelBase
	, IDisposable
{
	public VmIme Ime { get; }
	public IClipboardService ClipboardService { get; } = Di.GetRSvc<IClipboardService>();
	public IKeyboardHost KeyboardHost { get; } = Di.GetRSvc<IKeyboardHost>();

	public ObservableCollection<VmClipboardItem> Items{
		get => field;
		set => SetProperty(ref field, value);
	} = [];

	readonly PropertyChangedEventHandler _imePropertyChangedHandler;

	public VmClipboard(VmIme ime){
		Ime = ime;
		_imePropertyChangedHandler = async (_, e) => {
			if(e.PropertyName == nameof(VmIme.ShowClipboard) && ime.ShowClipboard){
				await RefreshAsy();
			}
		};
		ime.PropertyChanged += _imePropertyChangedHandler;
	}

	public async Task RefreshAsy(CT ct = default){
		var texts = await ClipboardService.GetItemsAsy(ct);
		var items = new ObservableCollection<VmClipboardItem>();
		foreach(var text in texts.Distinct()){
			items.Add(new VmClipboardItem{
				Text = text,
				Click = () => {
					KeyboardHost.CommitText(text);
					Ime.ExitClipboard();
				}
			});
		}
		Items = items;
	}

	public void Dispose()
	{
		Ime.PropertyChanged -= _imePropertyChangedHandler;
	}
}

public class VmClipboardItem : ViewModelBase
{
	public str Text{
		get => field;
		set => SetProperty(ref field, value);
	} = "";

	public Action? Click { get; set; }
}
