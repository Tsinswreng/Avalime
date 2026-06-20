using Avalime.UI.Infra;
using Avalime.UI.ViewModels;
using Avalime.UI.Views.candidatesBar;
using Avalime.UI.Views.clipboard;
using Avalime.UI.Views.KeyBoard;
using Avalime.UI.Views.preedit;
using Avalime.UI.Views.RimeLog;
using Avalime.UI.Views.toolbar;
using Avalonia.Controls;
using System.ComponentModel;
using Avalime.Core.Infra;
using Avalime.Core.Keys;

namespace Avalime.UI.Views;

public class ViewIme : AppViewBase<VmIme>
	, IDisposable
{
	ViewPreedit? _preedit;
	ViewToolBar? _toolbar;
	ViewCandidatesBar? _candidates;
	ViewKeyBoard? _keyboard;
	ViewClipboard? _clipboard;
	ViewRimeLog? _rimeLog;
	PropertyChangedEventHandler? _ctxPropertyChangedHandler;

	public ViewIme(){
		Ctx = new VmIme(
			Di.GetRSvc<ImeState>()
			, Di.GetRSvc<RimeConnectionState>()
		);
		Render();
	}

	void Render(){
		var preeditHeight = UiCfg.Inst.PreeditHeight;
		var topBarHeight = UiCfg.Inst.TopBarHeight;
		var root = new Grid{
			RowDefinitions = new($"{preeditHeight},{topBarHeight},*")
		};

		var preedit = new ViewPreedit();
		_preedit = preedit;
		preedit.Height = preeditHeight;
		Grid.SetRow(preedit, 0);

		var barHost = new Grid{
			Height = topBarHeight
		};
		var toolbar = new ViewToolBar(new VmToolBar(Ctx!, Di.GetRSvc<RimeConnectionState>()));
		_toolbar = toolbar;
		var candidates = new ViewCandidatesBar(
			new VmCandidatesBar(
				Di.GetRSvc<ImeState>()
				, Di.GetRSvc<RimeConnectionState>()
			)
		);
		_candidates = candidates;
		toolbar.Height = topBarHeight;
		candidates.Height = topBarHeight;
		barHost.Children.Add(toolbar);
		barHost.Children.Add(candidates);
		Grid.SetRow(barHost, 1);

		var bodyHost = new Grid();
		var keyboard = new ViewKeyBoard();
		_keyboard = keyboard;
		var clipboard = new ViewClipboard(
			new VmClipboard(
				Ctx!
				, Di.GetRSvc<IClipboardService>()
				, Di.GetRSvc<IKeyboardHost>()
			)
		);
		_clipboard = clipboard;
		var rimeLog = new ViewRimeLog(new VmRimeLog(Di.GetRSvc<RimeLogBuffer>()));
		_rimeLog = rimeLog;
		bodyHost.Children.Add(keyboard);
		bodyHost.Children.Add(clipboard);
		bodyHost.Children.Add(rimeLog);
		Grid.SetRow(bodyHost, 2);

		void SyncVisible(){
			preedit.IsVisible = Ctx!.ShowPreedit;
			preedit.IsHitTestVisible = Ctx.ShowPreedit;

			toolbar.IsVisible = Ctx.ShowToolbar;
			toolbar.IsHitTestVisible = Ctx.ShowToolbar;

			candidates.IsVisible = Ctx.ShowCandidates;
			candidates.IsHitTestVisible = Ctx.ShowCandidates;

			keyboard.IsVisible = Ctx.ShowKeyboard;
			keyboard.IsHitTestVisible = Ctx.ShowKeyboard;

			clipboard.IsVisible = Ctx.ShowClipboard;
			clipboard.IsHitTestVisible = Ctx.ShowClipboard;

			rimeLog.IsVisible = Ctx.ShowRimeLog;
			rimeLog.IsHitTestVisible = Ctx.ShowRimeLog;
		}

		_ctxPropertyChangedHandler = (_, e) => {
			if(
				e.PropertyName is nameof(Ctx.ShowPreedit)
				or nameof(Ctx.ShowToolbar)
				or nameof(Ctx.ShowCandidates)
				or nameof(Ctx.ShowKeyboard)
				or nameof(Ctx.ShowClipboard)
				or nameof(Ctx.ShowRimeLog)
			){
				SyncVisible();
			}
		};
		Ctx.PropertyChanged += _ctxPropertyChangedHandler;
		SyncVisible();

		root.Children.Add(preedit);
		root.Children.Add(barHost);
		root.Children.Add(bodyHost);
		this.SetContent(root);
	}

	public void Dispose()
	{
		if(_ctxPropertyChangedHandler is not null && Ctx is not null){
			Ctx.PropertyChanged -= _ctxPropertyChangedHandler;
		}
		(_preedit as IDisposable)?.Dispose();
		(_toolbar as IDisposable)?.Dispose();
		(_candidates as IDisposable)?.Dispose();
		_keyboard?.Dispose();
		(_clipboard as IDisposable)?.Dispose();
		(_rimeLog as IDisposable)?.Dispose();
		(Ctx as IDisposable)?.Dispose();
	}
}
