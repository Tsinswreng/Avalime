using Avalime.Core.Keys;
using Avalime.Core.Infra;
using Avalime.Rime;
using Avalime.ViewModels;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using Rime.Api;
using System.ComponentModel;
using Tsinswreng.CsInterop;

namespace Avalime.UI.ViewModels;

public class VmIme : ViewModelBase
	, IDisposable
{
	public static VmIme Mk(){return new VmIme();}

	public ImeState ImeState { get; } = Di.GetRSvc<ImeState>();
	public RimeConnectionState RimeConnection { get; } = Di.GetRSvc<RimeConnectionState>();

	public bool IsComposing{
		get => field;
		set => SetProperty(ref field, value);
	}

	public bool IsClipboardVisible{
		get => field;
		set => SetProperty(ref field, value);
	}

	public bool HasPreedit{
		get => field;
		set => SetProperty(ref field, value);
	}

	public bool ShowToolbar => !IsComposing || IsClipboardVisible;
	public bool ShowCandidates => (IsComposing || HasPreedit) && !IsClipboardVisible;
	public bool ShowPreedit => !IsClipboardVisible;
	public bool ShowKeyboard => !IsClipboardVisible;
	public bool ShowClipboard => IsClipboardVisible;

	readonly PropertyChangedEventHandler _propertyChangedHandler;
	readonly EventHandler<IEnumerable<IKeyEvent>> _afterInputHandler;

	public VmIme(){
		_propertyChangedHandler = (_, e) => {
			if(
				e.PropertyName is nameof(IsComposing)
				or nameof(HasPreedit)
				or nameof(IsClipboardVisible)
			){
				OnPropertyChanged(nameof(ShowToolbar));
				OnPropertyChanged(nameof(ShowCandidates));
				OnPropertyChanged(nameof(ShowPreedit));
				OnPropertyChanged(nameof(ShowKeyboard));
				OnPropertyChanged(nameof(ShowClipboard));
			}
		};
		PropertyChanged += _propertyChangedHandler;

		_afterInputHandler = (_, _) => RefreshCompositionState();
		ImeState.AfterInput += _afterInputHandler;
		_ = RimeConnection.ConnectAsy();
	}

	unsafe void RefreshCompositionState(){
		var rime = RimeConnection.Setup;
		if(rime is null){
			Dispatcher.UIThread.Post(() => {
				IsComposing = false;
				HasPreedit = false;
			});
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

		var ctx = new RimeContext{
			data_size = RimeUtil.DataSize<RimeContext>()
		};
		var gotContext = rime.apiFn.get_context(rime.rimeSessionId, &ctx) == RimeUtil.True;
		var hasPreedit = false;
		if(gotContext){
			hasPreedit = ctx.composition.length > 0 || !string.IsNullOrEmpty(ToolCStr.ToCsStr(ctx.composition.preedit));
			rime.apiFn.free_context(&ctx);
		}

		Dispatcher.UIThread.Post(() => {
			IsComposing = composing;
			HasPreedit = hasPreedit;
		});
	}

	public void ToggleClipboard(){
		IsClipboardVisible = !IsClipboardVisible;
	}

	public void ExitClipboard(){
		IsClipboardVisible = false;
	}

	public void Dispose()
	{
		PropertyChanged -= _propertyChangedHandler;
		ImeState.AfterInput -= _afterInputHandler;
	}
}
