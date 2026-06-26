using Avalime.Core.Infra;
using Avalime.UI.Infra;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;

namespace Avalime.UI.Views.Input;
using Ctx = VmInput;

public class ViewInput : AppViewBase<Ctx>
	, IDisposable
{
	public ViewInput(){
		Ctx = Di.DiOrMk<Ctx>();
		Render();
	}

	GridStack Root = new(IsRow: true);

	void Render(){
		var keyboardFont = UiCfg.Inst.KeyboardFontFamily;
		this.SetContent(Root.Grid);
		Root.Grid.Background = Brushes.Black;
		Root
		.A(new WrapPanel(), wp=>{
			wp.Background = Brushes.Black;
			wp.VerticalAlignment = VAlign.Center;
			wp.Margin = new (0);
			wp.A(new TextBlock(), o=>{
				if(keyboardFont is not null) o.FontFamily = keyboardFont;
				o.Foreground = Brushes.White;
				o.FontSize = UiCfg.Inst.PreeditFontSize;
				o.Margin = new(4, 1, 0, 1);
				o.VerticalAlignment = VAlign.Center;
				Ctx.Bind(o, x=>x.Text, x=>x.Text, Mode: BindingMode.TwoWay);
			});
		});
	}

	public void Dispose()
	{
		(Ctx as IDisposable)?.Dispose();
	}
}
