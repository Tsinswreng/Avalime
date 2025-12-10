using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia;

using Avalonia.Data;
using Tsinswreng.AvlnTools.Tools;

namespace Avalime.UI.Views.input;
using Ctx = Avalime.UI.Views.input.VmInput;
public class ViewInput : UserControl {
	public ViewInput() {
		ctx = new Ctx();
		_render();
	}


	public Ctx? ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	protected zero _render() {
		var ans = new Grid();
		Content = ans;
		{{
			var wrap = new WrapPanel();
			ans.Children.Add(wrap);
			{{
				var text = new TextBlock();
				wrap.Children.Add(text);
				{
					var o = text;
					o.Bind(
						TextBlock.TextProperty
						,new CBE(CBE.Pth<Ctx, str>(x=>x.Text)){
							Mode=BindingMode.TwoWay
						}
					);
				}
			}}//~wrap
		}}//~ans
		return 0;
	}
}

