namespace Avalime.UI.Views.input;
using Avalonia.Controls;
using Avalonia.Data;
using Avalime.UI.Infra;
using Ctx = VmInput;

public class ViewInput : AppViewBase<Ctx>
{
	public ViewInput(){
		Ctx = Ctx.Mk();
		Render();
	}

	GridStack Root = new(IsRow: true);

	void Render(){
		this.SetContent(Root.Grid);
		Root
		.A(new WrapPanel(), wp=>{
			wp.A(new TextBlock(), o=>{
				Ctx.Bind(o, x=>x.Text, x=>x.Text, Mode: BindingMode.TwoWay);
			});
		})
		;
	}
}
