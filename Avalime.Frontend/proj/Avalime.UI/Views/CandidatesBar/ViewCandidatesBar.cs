using Avalime.Core.Infra;
using Avalime.UI.Infra;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;
using ViewCandidateControl = Avalime.UI.Views.Candidate.ViewCandidate;
using VmCandidateCtx = Avalime.UI.Views.Candidate.VmCandidate;

namespace Avalime.UI.Views.CandidatesBar;
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
		ans.SetItemTemplate<VmCandidateCtx>((vm, ns)=>{
			var view = new ViewCandidateControl();
			view.CBind<VmCandidateCtx>(DataContextProperty, x=>x);
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

