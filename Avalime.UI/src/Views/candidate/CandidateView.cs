using System.Collections.Generic;
using Avalime.UI.controls;
using Avalime.UI.Ext;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Styling;
using Shr.Avalonia;

namespace Avalime.UI.views.candidate;
using Ctx = CandidateVm;
public class CandidateView:UserControl{


	public Ctx? ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}
	public CandidateView(){
		//ctx = new Ctx();
		ctx = Ctx.samples[0];
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
			o.set(
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
			o.set(
				MarginProperty
				,new Thickness(0)
			);
			o.set(
				PaddingProperty
				,new Thickness(0)
			);
			o.set(
				VerticalAlignmentProperty
				,VerticalAlignment.Stretch
			);
			o.set(
				HorizontalAlignmentProperty
				,HorizontalAlignment.Stretch
			);
			o.set(
				CornerRadiusProperty
				,new CornerRadius(0)
			);
		}

		var text = new Style(x=>
			x.Is<Control>()
			.Class(cls.Text)
		);
		Styles.Add(text);
		{
			var o = text;
			o.set(
				FontSizeProperty
				,20.0
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
						,new CBE(CBE.pth<Ctx>(x=>x.comment))
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
						,new CBE(CBE.pth<Ctx>(x=>x.text))
					);
				}
			}}
		}}
		return 0;
	}


}