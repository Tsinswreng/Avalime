using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

public class TopBar : UserControl{
	public TopBar(){
		_render();
	}

	protected zero _render(){
		var ans = new Grid();
		Content = ans;
		{{
			var testSwipBtn = new Button{Content="swipe"};
			ans.Children.Add(testSwipBtn);
			{
				var o = testSwipBtn;
				o.OnPointerPressed


			}
		}}//~ans
		return 0;
	}
}

public class SwipeButton : Button {
    // 注册自定义路由事件
    public static readonly RoutedEvent<SwipeEventArgs> SwipeEvent =
        RoutedEvent.Register<SwipeButton, SwipeEventArgs>(
            nameof(Swipe), RoutingStrategies.Bubble);

    // 事件包装器
    public event EventHandler<SwipeEventArgs> Swipe {
        add => AddHandler(SwipeEvent, value);
        remove => RemoveHandler(SwipeEvent, value);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e) {
        base.OnPointerPressed(e);
        // 记录起始位置...
    }

    protected override void OnPointerMoved(PointerEventArgs e) {
        base.OnPointerMoved(e);
        // 计算滑动并触发事件...
        var args = new SwipeEventArgs() {//SwipeEvent
            Direction = SwipeDirection.Right,
            Distance = 50
        };
        RaiseEvent(args);
    }
}

// 自定义事件参数
public class SwipeEventArgs : RoutedEventArgs {
    public SwipeDirection Direction { get; set; }
    public double Distance { get; set; }
}

public enum SwipeDirection {
    Left, Right, Up, Down
}


