using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia;

namespace Avalime.controls;
public class SwipeButton : Button {
	private Point _startPoint;
	private bool _isSwiping;

	// 定义自定义滑动事件
	public event EventHandler<SwipeEventArgs> Swipe;

	public SwipeButton() {
		// 绑定指针事件
		PointerPressed += _onPointerPressed;
		PointerMoved += _onPointerMoved;
		PointerReleased += _onPointerReleased;
	}

	protected override void OnPointerPressed(PointerPressedEventArgs e){
		base.OnPointerPressed(e);
		//System.Console.WriteLine("override pressed");//t +
		_onPointerPressed(this, e);
	}



	private void _onPointerPressed(object sender, PointerPressedEventArgs e) {
		//System.Console.WriteLine("pressed");//t -
		_startPoint = e.GetPosition(this); // 记录初始触摸点
		_isSwiping = true;
		e.Handled = true; // 阻止事件冒泡
	}

	private void _onPointerMoved(object sender, PointerEventArgs e) {
		//System.Console.WriteLine("moved");//t +
		if (!_isSwiping) return;
		var currentPoint = e.GetPosition(this);
		var delta = currentPoint - _startPoint;

		// 判断滑动方向（可调整阈值）
		if (Math.Abs(delta.X) > 50 || Math.Abs(delta.Y) > 50) {
			var direction = _getSwipeDirection(delta);
			Swipe?.Invoke(this, new SwipeEventArgs(direction));
			_isSwiping = false; // 触发后结束滑动
		}
	}

	private void _onPointerReleased(object sender, PointerReleasedEventArgs e) {
		//System.Console.WriteLine("released");//t
		_isSwiping = false;
	}

	private SwipeDirection _getSwipeDirection(Vector delta) {
		if (Math.Abs(delta.X) > Math.Abs(delta.Y)) {
			return delta.X > 0 ? SwipeDirection.Right : SwipeDirection.Left;
		} else {
			return delta.Y > 0 ? SwipeDirection.Down : SwipeDirection.Up;
		}
	}
}

// 自定义事件参数
public class SwipeEventArgs : EventArgs {
	public SwipeDirection Direction { get; }

	public SwipeEventArgs(SwipeDirection direction) {
		Direction = direction;
	}
}

public enum SwipeDirection {
	Up,
	Down,
	Left,
	Right
}
