using Avalime.UI.Views.Candidate;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;

namespace Avalime.UI.Views.candidatesBar;
using Ctx = VmCandidatesBar;
public partial class ViewCandidatesBar:UserControl{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}


	public ViewCandidatesBar(){
		Ctx = new Ctx();
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
				,CBE.Mk<Ctx>(x=>x.CandVms)
			);
			o.ItemsPanel = new FuncTemplate<Panel?>(()=>{
				return new StackPanel(){
					Orientation = Orientation.Horizontal
					,Spacing = 2.0
				};
			});


		}
		ans.ItemTemplate = new FuncDataTemplate<VmCandidate>((vm,b)=>{
			var ans2 = new ViewCandidate();
			{
				var o = ans2;
				o.Bind(
					DataContextProperty
					,CBE.Mk<VmCandidate>(x=>x)
				);
			}
			return ans2;
		});
		return ans;
	}


}
