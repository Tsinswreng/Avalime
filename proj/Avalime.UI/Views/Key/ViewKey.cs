//ViewKey: 單個鍵盤按鍵視圖、樣式匹配 TswG 暗色方案
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

	/// TswG 暗色配色常量
	/// 來源: TswG-3.3.10.trime.yaml preset_color_schemes.TswG
	static class TswG{
		public static readonly SolidColorBrush KeyBg = Brush("#000000");       //按鍵背景黑 (off_key_back_color)
		public static readonly SolidColorBrush KeyText = Brush("#E0E0E0");    //按鍵文字淺灰 (key_text_color)
		public static readonly SolidColorBrush KeyBorder = Brush("#ECEFF1");  //按鍵邊框淺灰 (key_border_color)
		public static readonly SolidColorBrush OnKeyBg = Brush("#4DB6AC");    //按下時背景青 (on_key_back_color)
		public static readonly SolidColorBrush OnKeyText = Brush("#37474F");  //按下時文字深灰 (on_key_text_color)
		public static readonly SolidColorBrush HiKeyBg = Brush("#4DB6AC");    //高亮按鍵背景青 (hilited_key_back_color)
		public static readonly SolidColorBrush HiKeyText = Brush("#000000");  //高亮按鍵文字黑 (hilited_key_text_color)
		public static readonly CornerRadius Round = new(0);                   //無圓角 (round_corner: 0)
		public static readonly Thickness KeyMargin = new(0);                 //按鍵緊密相連、無縫隙
		public static readonly Thickness BorderThick = new(0.5);             //邊框即爲視覺分隔線
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
