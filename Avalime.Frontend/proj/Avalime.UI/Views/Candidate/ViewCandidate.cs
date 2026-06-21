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
	Border _border = default!;
	bool _isPressedInside;

	public ViewCandidate(){
		Ctx = Ctx.Mk();
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
		_border = border;
		Ctx.Bind(border, Border.BackgroundProperty, x=>x.Background);
		border.PointerPressed += (_, e) => {
			_isPressedInside = true;
			e.Pointer.Capture(border);
		};
		border.PointerReleased += (_, e) => {
			var shouldClick = _isPressedInside && IsPointerInside(e.GetPosition(_border));
			_isPressedInside = false;
			if(e.Pointer.Captured == border){
				e.Pointer.Capture(null);
			}
			if(shouldClick){
				Ctx?.Click?.Invoke();
			}
		};
		border.PointerCaptureLost += (_, _) => {
			_isPressedInside = false;
		};

		this.SetContent(border);
		Root.Grid.Height = UiCfg.Inst.TopBarHeight;
		Root.SetRowDefs([
			new(UiCfg.Inst.CandidateCommentHeight, GUT.Pixel),
			new(UiCfg.Inst.CandidateTextHeight, GUT.Pixel),
		]);

		Root
		.A(new TextBlock(), o=>{
			o.Classes.Add(Cls.Comment);
			if(keyboardFont is not null) o.FontFamily = keyboardFont;
			o.VerticalAlignment = VAlign.Top;
			o.HorizontalAlignment = HAlign.Center;
			o.TextAlignment = TxtAlign.Center;
			o.Margin = new(0);
			o.Padding = new Thickness(0);
			o.TextTrimming = TextTrimming.None;
			Ctx.Bind(o, x=>x.Text, x=>x.Comment);
			Ctx.Bind(o, x=>x.Foreground, x=>x.Foreground);
		})
		.A(new Grid(), row=>{
			row.VerticalAlignment = VAlign.Stretch;
			row.HorizontalAlignment = HAlign.Stretch;
			row.Margin = new Thickness(0);
			row.A(new TextBlock(), o=>{
				o.Classes.Add(Cls.Text);
				if(keyboardFont is not null) o.FontFamily = keyboardFont;
				o.VerticalAlignment = VAlign.Bottom;
				o.HorizontalAlignment = HAlign.Center;
				o.TextAlignment = TxtAlign.Center;
				o.Margin = new(0);
				o.Padding = new Thickness(0);
				Ctx.Bind(o, x=>x.Text, x=>x.Text);
				Ctx.Bind(o, x=>x.Foreground, x=>x.Foreground);
			});
		})
		;
	}

	bool IsPointerInside(Point p)
	{
		return p.X >= 0 && p.X <= _border.Bounds.Width
			&& p.Y >= 0 && p.Y <= _border.Bounds.Height;
	}
}

