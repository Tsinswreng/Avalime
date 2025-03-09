using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia;
using Avalime.UI.controls;


namespace Avalime.UI.views.topBar;
public class TopBar : UserControl {
	public TopBar() {
		_render();
	}

	protected zero _render() {
		var ans = new Grid();
		Content = ans;
		{{
			var wrap = new WrapPanel();
			ans.Children.Add(wrap);
			{{
				var testLongPressBtn = new LongPressBtn { Content = "longPress" };
				wrap.Children.Add(testLongPressBtn);
				{
					var o = testLongPressBtn;
					// o.Swipe += (sender, e) => {
					// 	System.Console.WriteLine(e.Direction);
					// };
					o.Click += (sender, e) => {
						System.Console.WriteLine("click");
					};
					o.LongPressed += (sender, e) => {
						System.Console.WriteLine("long press");
					};
				}

				var swipeBtn = new SwipeBtn { Content = "swipe" };
				wrap.Children.Add(swipeBtn);
				{
					var o = swipeBtn;
					o.Swipe += (sender, e) => {
						System.Console.WriteLine(e.Direction);
					};
					o.Click += (sender, e) => {
						System.Console.WriteLine("click");
					};
				}

				var both = new SwipeLongPressBtn { Content = "both" };
				wrap.Children.Add(both);
				{
					var o = both;
					o.Swipe += (sender, e) => {
						System.Console.WriteLine(e.Direction);
					};
					o.Click += (sender, e) => {
						System.Console.WriteLine("click");
					};
					o.LongPressed += (sender, e) => {
						System.Console.WriteLine("long press");
					};
				}
			}}//~wrap
		}}//~ans
		return 0;
	}
}

