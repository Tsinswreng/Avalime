namespace Avalime.UI.Views.Candidate;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using Tsinswreng.AvlnTools.Controls;
using Tsinswreng.AvlnTools.Tools;
using Ctx = VmCandidate;


public class ViewCandidate:UserControl{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}
	public ViewCandidate(){
		//ctx = new Ctx();
		Ctx = Ctx.Samples[0];
		_style();
		_render();
	}

	public class Cls{
		public str Text=nameof(Cls.Text);
		public str Comment=nameof(Cls.Comment);
	}
	public Cls cls{get;} = new Cls();

	protected zero _style(){

		var grid = new Style(x=>
			x.Is<Grid>()
		);
		//Styles.Add(grid);
		{
			var o = grid;
			o.Set(
				Grid.ShowGridLinesProperty
				,true
			);
		}
		var btn = new Style(x=>
			Selectors.Or(
				x.Is<Button>()
				,x.Is<Control>()
			)
		);
		Styles.Add(btn);
		{
			var o = btn;
			o.Set(
				MarginProperty
				, new Thickness(0)
			);
			o.Set(
				PaddingProperty
				, new Thickness(0)
			);
			o.Set(
				VerticalAlignmentProperty
				, VAlign.Stretch
			);
			o.Set(
				HorizontalAlignmentProperty
				, HAlign.Stretch
			);
			o.Set(
				CornerRadiusProperty
				, new CornerRadius(0)
			);
		}

		var text = new Style(x=>
			x.Is<Control>()
			.Class(cls.Text)
		);
		Styles.Add(text);
		{
			var o = text;
			o.Set(
				FontSizeProperty
				, 20.0
			);

		}
		return 0;
	}

	protected zero _render(){
		var container = new SwipeLongPressBtn();
		Content = container;
		{{
			var grid = new Grid();
			container.Content = grid;
			{
				var o = grid;
				o.RowDefinitions.AddRange([
					new RowDefinition(1, GridUnitType.Star)
					,new RowDefinition(4, GridUnitType.Star)
					,new RowDefinition(1, GridUnitType.Star)
				]);
			}
			{{
				var comment = new TextBlock();
				grid.Children.Add(comment);
				{
					var o = comment;
					Grid.SetRow(o, 0);
					o.Classes.Add(cls.Comment);
					o.Bind(
						TextBlock.TextProperty
						,CBE.Mk<Ctx>(x=>x.Comment)
					);
				}

				var text = new TextBlock();
				grid.Children.Add(text);
				{
					var o = text;
					Grid.SetRow(o, 1);
					o.Classes.Add(cls.Text);
					o.Bind(
						TextBlock.TextProperty
						,new CBE(CBE.Pth<Ctx>(x=>x.Text))
					);
				}
			}}
		}}
		return 0;
	}


}
