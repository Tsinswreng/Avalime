//候選欄：水平滾動的候選詞列表
using Avalime.UI.Views.Candidate;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;
using Avalime.UI.Infra;

namespace Avalime.UI.Views.candidatesBar;
using Ctx = VmCandidatesBar;

public class ViewCandidatesBar : AppViewBase<Ctx>
{
	public ViewCandidatesBar(){
		Ctx = Ctx.Mk();
		Render();
	}

	GridStack Root = new(IsRow: true);

	void Render(){
		Root.Grid.Background = Brushes.Black;
		Root.Grid.Height = UiCfg.Inst.TopBarHeight;
		this.SetContent(Root.Grid);
		var Items = MkItems();
		Root.A(Items);
	}

	ItemsControl MkItems(){
		var R = new ItemsControl();
		R.SetItemTemplate<VmCandidate>((vm, ns)=>{
			var v = new ViewCandidate();
			v.CBind<VmCandidate>(DataContextProperty, x=>x);
			return v;
		}).SetItemsPanel(()=>new StackPanel{
			Orientation = Orientation.Horizontal,
			Spacing = 2.0
		});
		Ctx.Bind(R, ItemsControl.ItemsSourceProperty, x=>x.CandVms);
		return R;
	}
}
