using System;
using System.Linq.Expressions;
using System.Reactive.Linq;
using Avalime.UI.Ext;
using Avalime.ViewModels.key;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Data;
using Avalonia.Data.Core;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Markup.Xaml.MarkupExtensions.CompiledBindings;
using Avalonia.Media;
using Avalonia.Styling;
using Shr.Avalonia;
using Shr.Avalonia.util;
using BaseBtn = Avalonia.Controls.Button;
using Button = Avalime.UI.controls.SwipeLongPressBtn;
namespace Avalime.Views.Key;

public partial class KeyView : UserControl {
	public KeyView() {
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
			o.set(
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
			// o.set(
			// 	BorderBrushProperty
			// 	,Brushes.Aqua
			// );
		}

		var btnPointerover = new Style(x=>
			x.Is<BaseBtn>()
			.Class(PsdCls.inst.pointerover)
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
			.Class(PsdCls.inst.pressed)
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
		{
			var o = btn;
			o.Click += (s,e)=>{
				ctx?.click?.Invoke();
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
							,new CBE(CBE.pth<KeyVm, str>(x=>x.label))
						);
					}//
				}}//~keyBorder:Border
			}}//~container
		}}//~btn
		return 0;
	}
}
