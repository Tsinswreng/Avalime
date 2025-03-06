using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia;
using Avalime.controls;



public class TopBar : UserControl {
	public TopBar() {
		_render();
	}

	protected zero _render() {
		var ans = new Grid();
		Content = ans;
		{
			{
				var testSwipBtn = new LongPressButton { Content = "swipe" };
				ans.Children.Add(testSwipBtn);
				{
					var o = testSwipBtn;
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
			}
		}//~ans
		return 0;
	}
}

