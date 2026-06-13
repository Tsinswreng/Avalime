//ViewKey: 單個鍵盤按鍵視圖
using Avalime.ViewModels.key;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using Avalime.UI.Infra;
using BaseBtn = Avalonia.Controls.Button;

namespace Avalime.UI.Views.Key;
using Ctx = KeyVm;

public class ViewKey : AppViewBase<Ctx>
{
	public ViewKey(){
		Ctx = Ctx.Mk();
		Style();
		Render();
	}

	public class Cls{
		public const str Container = nameof(Container);
		public const str Label = nameof(Label);
		public const str LabelBorder = nameof(LabelBorder);
	}

	void Style(){
		Styles.A(
			Sty.Is<Control>()
			.Set(CornerRadiusProperty, new CornerRadius(0))
		).A(
			Sty.Is<BaseBtn>()
			.Set(MarginProperty, new Thickness(0))
			.Set(PaddingProperty, new Thickness(0))
			.Set(VerticalAlignmentProperty, VAlign.Stretch)
			.Set(HorizontalAlignmentProperty, HAlign.Stretch)
		).A(
			Sty.Is<Control>(x=>x.Class(Cls.Container))
			.Set(WidthProperty, 32.0)
			.Set(HeightProperty, 32.0)
		).A(
			Sty.Is<Control>(x=>x.Class(Cls.Label))
			.Set(MinHeightProperty, 0.0)
			.Set(MinWidthProperty, 0.0)
			.Set(HorizontalAlignmentProperty, HAlign.Center)
			.Set(VerticalAlignmentProperty, VAlign.Center)
			.Set(FontSizeProperty, 24.0)
		).A(
			Sty.Is<Border>(x=>x.Class(Cls.LabelBorder))
			.Set(BorderThicknessProperty, new Thickness(1))
			.Set(BorderBrushProperty, Brushes.Aqua)
			.Set(MarginProperty, new Thickness(0,4,0,4))
		);
	}

	void Render(){
		var Btn = new Button();
		Content = Btn;
		Btn.Click += (s,e)=>Ctx?.Click?.Invoke();

		Btn.SetContent(new StackPanel(), sp=>{
			sp.Classes.Add(Cls.Container);
			sp.A(new Border(), border=>{
				border.Classes.Add(Cls.LabelBorder);
				border.SetChild(new TextBlock(), o=>{
					o.Classes.Add(Cls.Label);
					o.Bind(TextBlock.TextProperty, CBE.Mk<Ctx>(x=>x.Label));
				});
			});
		});
	}
}
