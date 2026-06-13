//ViewKey: 單個鍵盤按鍵視圖、樣式匹配 TswG
using Avalime.ViewModels.key;
using Avalonia;
using Avalonia.Controls;
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
		public const str KeyBtn = nameof(KeyBtn);
	}

	/// TswG 配色常量
	static class TswG{
		public static readonly SolidColorBrush KeyBg = Brush("#FFFFFF");       //按鍵背景白
		public static readonly SolidColorBrush KeyText = Brush("#000000");     //按鍵文字黑
		public static readonly SolidColorBrush KeyBorder = Brush("#ECEFF1");   //按鍵邊框淺灰
		public static readonly SolidColorBrush FuncBg = Brush("#ACB2C2");      //功能鍵背景灰
		public static readonly SolidColorBrush FuncText = Brush("#FFFFFF");    //功能鍵文字白
		public static readonly SolidColorBrush EnterBg = Brush("#3266A0");     //回車鍵背景藍
		public static readonly SolidColorBrush EnterText = Brush("#FFFFFF");   //回車鍵文字白
		public static readonly SolidColorBrush SpaceBg = Brush("#FFFFFF");     //空格鍵背景白
		public static readonly SolidColorBrush LabelColor = Brush("#3266A0");  //標籤色藍
		public static readonly CornerRadius Round = new(12);          //圓角半徑
		public static readonly Thickness KeyMargin = new(2, 4);       //按鍵間距(H:2 V:4)
		public static readonly Thickness BorderThick = new(0.5);     //邊框粗細
	}

	static SolidColorBrush Brush(str Hex)=>SolidColorBrush.Parse(Hex);

	void Style(){
		Styles.A(
			Sty.Is<Control>()
			.Set(CornerRadiusProperty, TswG.Round)
		).A(
			Sty.Is<BaseBtn>(x=>x.Class(Cls.KeyBtn))
			.Set(MarginProperty, TswG.KeyMargin)
			.Set(PaddingProperty, new Thickness(0))
			.Set(VerticalAlignmentProperty, VAlign.Stretch)
			.Set(HorizontalAlignmentProperty, HAlign.Stretch)
			.Set(BackgroundProperty, TswG.KeyBg)
			.Set(BorderBrushProperty, TswG.KeyBorder)
			.Set(BorderThicknessProperty, TswG.BorderThick)
		).A(
			Sty.Is<Control>(x=>x.Class(Cls.Label))
			.Set(MinHeightProperty, 0.0)
			.Set(MinWidthProperty, 0.0)
			.Set(HorizontalAlignmentProperty, HAlign.Center)
			.Set(VerticalAlignmentProperty, VAlign.Center)
			.Set(FontSizeProperty, 24.0)
			.Set(ForegroundProperty, TswG.KeyText)
		).A(
			Sty.Is<Border>(x=>x.Class(Cls.LabelBorder))
			.Set(BorderThicknessProperty, new Thickness(0))
			.Set(BorderBrushProperty, Brushes.Transparent)
			.Set(MarginProperty, new Thickness(0, 2, 0, 2))
		);
	}

	void Render(){
		var Btn = new Button();
		Btn.Classes.Add(Cls.KeyBtn);
		Content = Btn;
		Btn.Click += (s,e)=>Ctx?.Click?.Invoke();

		Btn.SetContent(new Border(), border=>{
			border.Classes.Add(Cls.LabelBorder);
			border.SetChild(new TextBlock(), o=>{
				o.Classes.Add(Cls.Label);
				Ctx.Bind(o, x=>x.Text, x=>x.Label);
			});
		});
	}
}
