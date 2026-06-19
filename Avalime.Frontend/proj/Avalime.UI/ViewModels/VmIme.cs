using Avalime.Core.Keys;
using Avalime.Rime;
using Avalime.ViewModels;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using Rime.Api;

namespace Avalime.UI.ViewModels;

public class VmIme : ViewModelBase
{
	public static VmIme Mk(){return new VmIme();}

	public ImeState ImeState { get; } = App.SvcP.GetRequiredService<ImeState>();
	public RimeConnectionState RimeConnection { get; } = App.SvcP.GetRequiredService<RimeConnectionState>();

	public bool IsComposing{
		get => field;
		set => SetProperty(ref field, value);
	}

	public bool IsClipboardVisible{
		get => field;
		set => SetProperty(ref field, value);
	}

	public bool ShowToolbar => !IsComposing || IsClipboardVisible;
	public bool ShowCandidates => IsComposing && !IsClipboardVisible;
	public bool ShowPreedit => !IsClipboardVisible;
	public bool ShowKeyboard => !IsClipboardVisible;
	public bool ShowClipboard => IsClipboardVisible;

	public VmIme(){
		PropertyChanged += (_, e) => {
			if(
				e.PropertyName is nameof(IsComposing)
				or nameof(IsClipboardVisible)
			){
				OnPropertyChanged(nameof(ShowToolbar));
				OnPropertyChanged(nameof(ShowCandidates));
				OnPropertyChanged(nameof(ShowPreedit));
				OnPropertyChanged(nameof(ShowKeyboard));
				OnPropertyChanged(nameof(ShowClipboard));
			}
		};

		ImeState.AfterInput += (_, _) => RefreshCompositionState();
		_ = RimeConnection.ConnectAsy();
	}

	unsafe void RefreshCompositionState(){
		var rime = RimeConnection.Setup;
		if(rime is null){
			Dispatcher.UIThread.Post(() => IsComposing = false);
			return;
		}
		var status = new RimeStatus{
			data_size = RimeUtil.DataSize<RimeStatus>()
		};
		var gotStatus = rime.apiFn.get_status(rime.rimeSessionId, &status) == RimeUtil.True;
		var composing = gotStatus && status.is_composing != RimeUtil.False;
		if(gotStatus){
			rime.apiFn.free_status(&status);
		}
		Dispatcher.UIThread.Post(() => IsComposing = composing);
	}

	public void ToggleClipboard(){
		IsClipboardVisible = !IsClipboardVisible;
	}

	public void ExitClipboard(){
		IsClipboardVisible = false;
	}
}
