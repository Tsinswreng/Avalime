namespace Avalime.UI.Views.input;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
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
		Root.Grid.Background = Brushes.Black;
		Root.Grid.Height = UiCfg.Inst.PreeditHeight;
		Root
		.A(new WrapPanel(), wp=>{
			wp.Background = Brushes.Black;
			wp.VerticalAlignment = VAlign.Center;
			wp.A(new TextBlock(), o=>{
				if(keyboardFont is not null) o.FontFamily = keyboardFont;
				o.Foreground = Brushes.White;
				o.FontSize = UiCfg.Inst.TopBarFontSize;
				o.Margin = new(4, 0, 0, 0);
				o.VerticalAlignment = VAlign.Center;
				Ctx.Bind(o, x=>x.Text, x=>x.Text, Mode: BindingMode.TwoWay);
			});
		})
		;
	}
}
