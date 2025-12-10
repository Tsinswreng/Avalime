using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia;
using Avalime.UI.Views.candidatesBar;
using Tsinswreng.AvlnTools.Controls;


namespace Avalime.UI.Views.topBar;
public class ViewTopBar : UserControl {
	public ViewTopBar() {
		_render();
	}

	protected zero _render() {
		Content = new ViewCandidatesBar();
		return 0;
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
					o.OnLongPressed += (sender, e) => {
						System.Console.WriteLine("long press");
					};
				}

				var swipeBtn = new SwipeBtn { Content = "swipe" };
				wrap.Children.Add(swipeBtn);
				{
					var o = swipeBtn;
					o.OnSwipe += (sender, e) => {
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
					o.OnSwipe += (sender, e) => {
						System.Console.WriteLine(e.Direction);
					};
					o.Click += (sender, e) => {
						System.Console.WriteLine("click");
					};
					o.OnLongPressed += (sender, e) => {
						System.Console.WriteLine("long press");
					};
				}
			}}//~wrap
		}}//~ans
		return 0;
	}
}

