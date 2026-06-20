using Avalime.Core.Infra;
using Avalime.UI.Infra;
using ViewCandidatesBarControl = Avalime.UI.Views.ViewCandidatesBar.ViewCandidatesBar;
using ViewClipboardControl = Avalime.UI.Views.ViewClipboard.ViewClipboard;
using ViewKeyBoardControl = Avalime.UI.Views.ViewKeyBoard.ViewKeyBoard;
using ViewPreeditControl = Avalime.UI.Views.ViewPreedit.ViewPreedit;
using ViewRimeLogControl = Avalime.UI.Views.ViewRimeLog.ViewRimeLog;
using ViewToolBarControl = Avalime.UI.Views.ViewToolBar.ViewToolBar;
using Avalonia.Controls;
using System.ComponentModel;
using Tsinswreng.Avln.Grid;

namespace Avalime.UI.Views.ViewIme;
using Ctx = VmIme;

public class ViewIme : AppViewBase<Ctx>
	, IDisposable
{
	ViewPreeditControl? _preedit;
	ViewToolBarControl? _toolbar;
	ViewCandidatesBarControl? _candidates;
	ViewKeyBoardControl? _keyboard;
	ViewClipboardControl? _clipboard;
	ViewRimeLogControl? _rimeLog;
	PropertyChangedEventHandler? _ctxPropertyChangedHandler;
	GridStack Root = new(IsRow: true);

	public ViewIme(){
		Ctx = Di.DiOrMk<Ctx>();
		Render();
	}

	void Render(){
		var preeditHeight = UiCfg.Inst.PreeditHeight;
		var topBarHeight = UiCfg.Inst.TopBarHeight;
		this.SetContent(Root.Grid);
		Root.SetRowDefs([
			new(preeditHeight, GUT.Pixel),
			new(topBarHeight, GUT.Pixel),
			new(1, GUT.Star),
		]);

		var preedit = new ViewPreeditControl();
		_preedit = preedit;
		preedit.Height = preeditHeight;

		var toolbar = new ViewToolBarControl();
		_toolbar = toolbar;
		toolbar.Height = topBarHeight;

		var candidates = new ViewCandidatesBarControl();
		_candidates = candidates;
		candidates.Height = topBarHeight;

		var keyboard = new ViewKeyBoardControl();
		_keyboard = keyboard;

		var clipboard = new ViewClipboardControl();
		_clipboard = clipboard;

		var rimeLog = new ViewRimeLogControl();
		_rimeLog = rimeLog;

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

		Root
		.A(preedit, o=>{
			Grid.SetRow(o, 0);
		})
		.A(new Grid{
			Height = topBarHeight
		}, o=>{
			Grid.SetRow(o, 1);
			o.A(toolbar);
			o.A(candidates);
		})
		.A(new Grid(), o=>{
			Grid.SetRow(o, 2);
			o.A(keyboard);
			o.A(clipboard);
			o.A(rimeLog);
		});
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
