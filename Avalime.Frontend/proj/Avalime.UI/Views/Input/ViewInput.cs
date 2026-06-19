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
		var keyboardFont = UiCfg.Inst.KeyboardFontFamily;
		this.SetContent(Root.Grid);
		Root
		.A(new WrapPanel(), wp=>{
			wp.A(new TextBlock(), o=>{
				if(keyboardFont is not null) o.FontFamily = keyboardFont;
				Ctx.Bind(o, x=>x.Text, x=>x.Text, Mode: BindingMode.TwoWay);
			});
		})
		;
	}
}
