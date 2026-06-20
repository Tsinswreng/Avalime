using Avalime.UI.Infra;
using Avalime.UI.Views.ViewCandidate;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;

namespace Avalime.UI.Views.ViewCandidatesBar;
using Ctx = VmCandidatesBar;

public class ViewCandidatesBar : AppViewBase<Ctx>
{
	public ViewCandidatesBar(){
		Ctx = Di.DiOrMk<Ctx>();
		Render();
	}

	GridStack Root = new(IsRow: true);

	void Render(){
		Root.Grid.Background = Brushes.Black;
		Root.Grid.Height = UiCfg.Inst.TopBarHeight;
		this.SetContent(Root.Grid);
		var items = MkItems();
		Root.A(items);
	}

	ItemsControl MkItems(){
		var ans = new ItemsControl();
		ans.VerticalAlignment = VAlign.Stretch;
		ans.SetItemTemplate<VmCandidate>((vm, ns)=>{
			var view = new ViewCandidate();
			view.CBind<VmCandidate>(DataContextProperty, x=>x);
			return view;
		}).SetItemsPanel(()=>new StackPanel{
			Orientation = Orientation.Horizontal,
			Spacing = 2.0,
			VerticalAlignment = VAlign.Stretch,
		});
		Ctx.Bind(ans, ItemsControl.ItemsSourceProperty, x=>x.CandVms);
		return ans;
	}
}
