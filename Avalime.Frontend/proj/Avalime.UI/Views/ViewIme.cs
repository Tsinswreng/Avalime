using Avalime.UI.Infra;
using Avalime.UI.ViewModels;
using Avalime.UI.Views.candidatesBar;
using Avalime.UI.Views.clipboard;
using Avalime.UI.Views.KeyBoard;
using Avalime.UI.Views.preedit;
using Avalime.UI.Views.toolbar;
using Avalonia.Controls;

namespace Avalime.UI.Views;

public class ViewIme : AppViewBase<VmIme>
{
	public ViewIme(){
		Ctx = VmIme.Mk();
		Render();
	}

	void Render(){
		var topBarHeight = UiCfg.Inst.TopBarHeight;
		var root = new Grid{
			RowDefinitions = new($"{topBarHeight},{topBarHeight},*")
		};

		var preedit = new ViewPreedit();
		preedit.Height = topBarHeight;
		Grid.SetRow(preedit, 0);

		var barHost = new Grid{
			Height = topBarHeight
		};
		var toolbar = new ViewToolBar(new VmToolBar(Ctx!));
		var candidates = new ViewCandidatesBar();
		toolbar.Height = topBarHeight;
		candidates.Height = topBarHeight;
		barHost.Children.Add(toolbar);
		barHost.Children.Add(candidates);
		Grid.SetRow(barHost, 1);

		var bodyHost = new Grid();
		var keyboard = new ViewKeyBoard();
		var clipboard = new ViewClipboard(new VmClipboard(Ctx!));
		bodyHost.Children.Add(keyboard);
		bodyHost.Children.Add(clipboard);
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
		}

		Ctx.PropertyChanged += (_, e) => {
			if(
				e.PropertyName is nameof(Ctx.ShowPreedit)
				or nameof(Ctx.ShowToolbar)
				or nameof(Ctx.ShowCandidates)
				or nameof(Ctx.ShowKeyboard)
				or nameof(Ctx.ShowClipboard)
			){
				SyncVisible();
			}
		};
		SyncVisible();

		root.Children.Add(preedit);
		root.Children.Add(barHost);
		root.Children.Add(bodyHost);
		this.SetContent(root);
	}
}
