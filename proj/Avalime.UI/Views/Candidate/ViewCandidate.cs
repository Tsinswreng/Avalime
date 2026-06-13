namespace Avalime.UI.Views.Candidate;
using Avalonia;
using Avalonia.Controls;
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
			.Set(FontSizeProperty, 20.0)
		);
	}

	void Render(){
		this.SetContent(Root.Grid);
		Root.SetRowDefs([
			new(1, GUT.Star),
			new(4, GUT.Star),
			new(1, GUT.Star),
		]);

		Root
		.A(new TextBlock(), o=>{
			o.Classes.Add(Cls.Comment);
			o.Bind(TextBlock.TextProperty, CBE.Mk<Ctx>(x=>x.Comment));
		})
		.A(new TextBlock(), o=>{
			o.Classes.Add(Cls.Text);
			o.Bind(TextBlock.TextProperty, CBE.Mk<Ctx>(x=>x.Text));
		})
		;
	}
}
