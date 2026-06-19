namespace Avalime.UI.Views.Candidate;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Styling;
using Avalime.UI.Infra;
using Ctx = VmCandidate;

public class ViewCandidate : AppViewBase<Ctx>
{
	public ViewCandidate(){
		Ctx = Ctx.Samples[0];
		Style();
		Render();
	}

	public class Cls{
		public const str Text = nameof(Text);
		public const str Comment = nameof(Comment);
	}

	GridStack Root = new(IsRow: true);

	void Style(){
		Styles.A(
			Sty.Is<Button>()
			.Set(MarginProperty, new Thickness(0))
			.Set(PaddingProperty, new Thickness(0))
			.Set(VerticalAlignmentProperty, VAlign.Stretch)
			.Set(HorizontalAlignmentProperty, HAlign.Stretch)
			.Set(CornerRadiusProperty, new CornerRadius(0))
		).A(
			Sty.Is<Control>()
			.Set(MarginProperty, new Thickness(0))
			.Set(PaddingProperty, new Thickness(0))
			.Set(VerticalAlignmentProperty, VAlign.Stretch)
			.Set(HorizontalAlignmentProperty, HAlign.Stretch)
			.Set(CornerRadiusProperty, new CornerRadius(0))
		).A(
			Sty.Is<Control>(x=>x.Class(Cls.Text))
			.Set(FontSizeProperty, UiCfg.Inst.CandidateFontSize)
		).A(
			Sty.Is<Control>(x=>x.Class(Cls.Comment))
			.Set(FontSizeProperty, UiCfg.Inst.CandidateCommentFontSize)
		);
	}

	void Render(){
		var keyboardFont = UiCfg.Inst.KeyboardFontFamily;
		var border = new Border{
			Child = Root.Grid,
			Background = UiCfg.Inst.CandidateBgColor,
			BorderThickness = new Thickness(0.5),
			BorderBrush = SolidColorBrush.Parse("#253238"),
		};
		Ctx.Bind(border, Border.BackgroundProperty, x=>x.Background);
		border.PointerPressed += (_, _) => {
			Ctx?.Click?.Invoke();
		};

		this.SetContent(border);
		Root.SetRowDefs([
			new(UiCfg.Inst.CandidateCommentHeight, GUT.Pixel),
			new(1, GUT.Star),
		]);

		Root
		.A(new TextBlock(), o=>{
			o.Classes.Add(Cls.Comment);
			if(keyboardFont is not null) o.FontFamily = keyboardFont;
			o.VerticalAlignment = VAlign.Top;
			o.HorizontalAlignment = HAlign.Center;
			o.TextAlignment = TxtAlign.Center;
			o.Height = UiCfg.Inst.CandidateCommentHeight;
			o.Margin = new(0, 1, 0, 0);
			Ctx.Bind(o, x=>x.Text, x=>x.Comment);
			Ctx.Bind(o, x=>x.Foreground, x=>x.Foreground);
		})
		.A(new TextBlock(), o=>{
			o.Classes.Add(Cls.Text);
			if(keyboardFont is not null) o.FontFamily = keyboardFont;
			o.VerticalAlignment = VAlign.Bottom;
			o.HorizontalAlignment = HAlign.Center;
			o.TextAlignment = TxtAlign.Center;
			o.Margin = new(0, 0, 0, 2);
			Ctx.Bind(o, x=>x.Text, x=>x.Text);
			Ctx.Bind(o, x=>x.Foreground, x=>x.Foreground);
		})
		;
	}
}
