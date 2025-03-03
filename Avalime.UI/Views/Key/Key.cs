using Avalime.UI.Ext;
using Avalime.ViewModels.Key;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;

namespace Avalime.Views.Key;

public partial class Key : UserControl {
	public Key() {
		//InitializeComponent();
		DataContext = new KeyVm();
		_style();
		_render();
	}

	public KeyVm? ctx{
		get{return DataContext as KeyVm;}
		set{DataContext = value;}
	}

	public class Cls{
		public str container=nameof(Cls.container);
		public str label=nameof(Cls.label);
		public str labelBorder=nameof(Cls.labelBorder);
	}
	public Cls cls{get;set;}=new Cls();

	protected zero _style(){

		var btn = new Style(x=>
			x.Is<Button>()
			// .Template()
			// .OfType<ContentControl>()
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


		}

		var container= new Style(x=>
			x.Is<Control>().Class(cls.container)
		);
		Styles.Add(container);
		{var o = container;
			o.set(
				Control.WidthProperty
				,32.0
			);
			o.set(
				Control.HeightProperty
				,32.0
			);
			// o.set(
			// 	Control.HorizontalAlignmentProperty
			// 	,HorizontalAlignment.Center
			// );
			// o.set(
			// 	Control.VerticalAlignmentProperty
			// 	,VerticalAlignment.Center
			// );
		}
		var labelBorder= new Style(x=>
			x.Is<Border>().Class(cls.labelBorder)
		);
		//Styles.Add(labelBorder);
		{
			var o = labelBorder;
			o.set(
				Border.BorderThicknessProperty
				,new Thickness(1)
			);
			o.set(
				Border.BorderBrushProperty
				,Brushes.Aqua
			);
			o.set(
				Border.MarginProperty
				,new Thickness(0,4,0,4)
			);
		}
		var label= new Style(x=>
			x.Is<Control>().Class(cls.label)
		);
		Styles.Add(label);
		{var o = label;
			o.set(
				Control.MinHeightProperty
				,0.0
			);
			o.set(
				Control.MinWidthProperty
				,0.0
			);
			o.set(
				Control.HorizontalAlignmentProperty
				,HorizontalAlignment.Center
			);
			o.set(
				Control.VerticalAlignmentProperty
				,VerticalAlignment.Center
			);
			o.set(
				FontSizeProperty
				,24.0
			);
		}

		return 0;
	}

	protected zero _render(){
		var btn = new Button();
		Content = btn;
		{{
			var container = new StackPanel();
			btn.Content = container;
			{
				var o = container;
				o.Classes.Add(cls.container);
			}
			{{//container
				var keyBorder = new Border();
				container.Children.Add(keyBorder);
				keyBorder.Classes.Add(cls.labelBorder);
				{

				}
				{{//keyBorder:Border
					//
					var label = new TextBlock();
					keyBorder.Child = label;
					{//conf btn:Button
						var o = label;
						o.Classes.Add(cls.label);
						o.Bind(
							TextBlock.TextProperty
							,new Binding(nameof(ctx.label))
						);
					}//~conf btn:Button
				}}//~keyBorder:Border
			}}//~container
			//
		}}//~btn
		return 0;
	}
}