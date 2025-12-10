using System.Reactive.Linq;
using Avalime.ViewModels.key;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using Tsinswreng.AvlnTools.Tools;
using BaseBtn = Avalonia.Controls.Button;
//using Button
namespace Avalime.UI.Views.Key;

public partial class ViewKey : UserControl {
	public ViewKey() {
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

		var noCornerRadius = new Style(x=>
			x.Is<Control>()
		);
		Styles.Add(noCornerRadius);
		{
			var o = noCornerRadius;
			o.Set(
				CornerRadiusProperty
				,new CornerRadius(0)
			);
		}

		var btn = new Style(x=>
			x.Is<BaseBtn>()
			//x.OfType<Button>()
			// .Template()
			// .OfType<ContentControl>()
		);
		Styles.Add(btn);
		{
			var o = btn;
			o.Set(
				MarginProperty
				,new Thickness(0)
			);
			o.Set(
				PaddingProperty
				,new Thickness(0)
			);
			o.Set(
				VerticalAlignmentProperty
				,VerticalAlignment.Stretch
			);
			o.Set(
				HorizontalAlignmentProperty
				,HorizontalAlignment.Stretch
			);
			// o.set(
			// 	BorderBrushProperty
			// 	,Brushes.Aqua
			// );
		}

		var btnPointerover = new Style(x=>
			x.Is<BaseBtn>()
			.Class(PsdCls.Inst.pointerover)
			.Template()
			.OfType<ContentPresenter>()
		);
		Styles.Add(btnPointerover);
		{
			var o = btnPointerover;
			// o.set(
			// 	BackgroundProperty
			// 	,Brushes.Yellow
			// );
		}

		var btnPressed = new Style(x=>
			x.Is<BaseBtn>()
			.Class(PsdCls.Inst.pressed)
			.Template()
			.OfType<ContentPresenter>()
		);
		Styles.Add(btnPressed);
		{
			var o = btnPressed;
			// o.set(
			// 	BackgroundProperty
			// 	,Brushes.Green
			// );
		}

		var container= new Style(x=>
			x.Is<Control>().Class(cls.container)
		);
		Styles.Add(container);
		{var o = container;
			o.Set(
				Control.WidthProperty
				,32.0
			);
			o.Set(
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
			o.Set(
				Border.BorderThicknessProperty
				,new Thickness(1)
			);
			o.Set(
				Border.BorderBrushProperty
				,Brushes.Aqua
			);
			o.Set(
				Border.MarginProperty
				,new Thickness(0,4,0,4)
			);
		}
		var label= new Style(x=>
			x.Is<Control>().Class(cls.label)
		);
		Styles.Add(label);
		{var o = label;
			o.Set(
				Control.MinHeightProperty
				,0.0
			);
			o.Set(
				Control.MinWidthProperty
				,0.0
			);
			o.Set(
				Control.HorizontalAlignmentProperty
				,HorizontalAlignment.Center
			);
			o.Set(
				Control.VerticalAlignmentProperty
				,VerticalAlignment.Center
			);
			o.Set(
				FontSizeProperty
				,24.0
			);
		}

		return 0;
	}

	protected zero _render(){
		var btn = new Button();
		Content = btn;
		{
			var o = btn;
			o.Click += (s,e)=>{
				ctx?.Click?.Invoke();
			};

		}
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
							,new CBE(CBE.Pth<KeyVm, str>(x=>x.Label))
						);
					}//
				}}//~keyBorder:Border
			}}//~container
		}}//~btn
		return 0;
	}
}
