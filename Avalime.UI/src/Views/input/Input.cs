using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia;
using Avalime.UI.controls;
using Shr.Avalonia;

namespace Avalime.UI.views.input;
using Ctx = Avalime.UI.views.input.InputVm;
public class Input : UserControl {
	public Input() {
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
						,new CBE(CBE.pth<Ctx, str>(x=>x.text))
					);
				}
			}}//~wrap
		}}//~ans
		return 0;
	}
}

