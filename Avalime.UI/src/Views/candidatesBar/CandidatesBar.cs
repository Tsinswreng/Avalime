using System.Collections;
using Avalime.UI.views.candidate;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Shr.Avalonia;

namespace Avalime.UI.views.candidatesBar;
using Ctx = CandidatesBarVm;
public partial class CandidatesBar:UserControl{

	public Ctx? ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}


	public CandidatesBar(){
		ctx = new Ctx();
		//ctx = Ctx.samples[0];
		_style();
		_render();
	}

	public class Cls{

	}
	public Cls cls{get;set;} = new Cls();

	protected zero _style(){

		return 0;
	}

	protected zero _render(){
		var container = new ScrollViewer();
		Content = container;
		{
			var o = container;
			o.HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Visible;
			//o.MaxWidth = 300.0;
		}
		{{
			var items = _items();
			container.Content = items;
		}}
		return 0;
	}

	protected Control _items(){
		var ans = new ItemsControl();
		{
			var o = ans;
			o.Bind(
				ItemsControl.ItemsSourceProperty
				,new CBE(CBE.pth<Ctx>(x=>x.candVms))
			);
			o.ItemsPanel = new FuncTemplate<Panel?>(()=>{
				return new StackPanel(){
					Orientation = Orientation.Horizontal
					,Spacing = 2.0
				};
			});


		}
		ans.ItemTemplate = new FuncDataTemplate<CandidateVm>((vm,b)=>{
			var ans2 = new CandidateView();
			{
				var o = ans2;
				o.Bind(
					DataContextProperty
					,new CBE(CBE.pth<CandidateVm>(x=>x))
				);
			}
			return ans2;
		});
		return ans;
	}


}