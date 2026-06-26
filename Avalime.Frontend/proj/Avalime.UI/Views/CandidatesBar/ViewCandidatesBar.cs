using Avalime.Core.Infra;
using Avalime.Core.Infra.Log;
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
	, IDisposable
{
	public ViewCandidatesBar(){
		Ctx = Di.DiOrMk<Ctx>();
		Render();
	}

	GridStack Root = new(IsRow: true);
	double _cachedMinWidth;

	void Render(){
		Root.Grid.Background = Brushes.Black;
		Root.Grid.Height = UiCfg.Inst.TopBarHeight;
		this.SetContent(Root.Grid);
		var items = MkItems();
		Root.A(items);
		this.LayoutUpdated += (_, _) => {
			var width = this.Bounds.Width;
			var cnt = Ctx.CandVms.Count;
			if(width <= 0){
				AppLog.Debug($"[CandMinWidth] SKIP width={width}");
				return;
			}
			// 間隔由 BorderThickness 提供(與鍵盤一致)，MinWidth = 容器寬度 / 10
			var minWidth = Math.Max(0, width / 10.0);
			AppLog.Debug($"[CandMinWidth] width={width:F1} cnt={cnt} minWidth={minWidth:F1} cached={_cachedMinWidth:F1}");
			//寬度沒變 且 VM們已經有正確值 才跳過
			if(Math.Abs(minWidth - _cachedMinWidth) < 0.5 && cnt > 0
				&& Math.Abs(Ctx.CandVms[0].MinWidth - minWidth) < 0.5){
				return;
			}
			_cachedMinWidth = minWidth;
			AppLog.Debug($"[CandMinWidth] SET MinWidth={minWidth:F1} on {cnt} VMs");
			foreach(var vm in Ctx.CandVms){
				vm.MinWidth = minWidth;
			}
		};
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
			Spacing = 0, // 間隔由 BorderThickness(0.5×2) 提供，與鍵盤一致
			VerticalAlignment = VAlign.Stretch,
		});
		Ctx.Bind(ans, ItemsControl.ItemsSourceProperty, x=>x.CandVms);
		return ans;
	}

	public void Dispose()
	{
		(Ctx as IDisposable)?.Dispose();
	}
}
