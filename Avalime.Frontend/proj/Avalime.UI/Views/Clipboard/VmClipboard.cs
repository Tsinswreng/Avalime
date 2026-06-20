using System.Collections.ObjectModel;
using System.ComponentModel;
using Avalime.UI.ViewModels;
using Avalime.ViewModels;

namespace Avalime.UI.Views.clipboard;

public class VmClipboard : ViewModelBase
	, IDisposable
{
	public VmIme Ime { get; }
	public IClipboardService ClipboardService { get; }
	public IKeyboardHost KeyboardHost { get; }

	public ObservableCollection<VmClipboardItem> Items{
		get => field;
		set => SetProperty(ref field, value);
	} = [];

	readonly PropertyChangedEventHandler _imePropertyChangedHandler;

	public VmClipboard(VmIme ime, IClipboardService ClipboardService, IKeyboardHost KeyboardHost){
		Ime = ime;
		this.ClipboardService = ClipboardService;
		this.KeyboardHost = KeyboardHost;
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
