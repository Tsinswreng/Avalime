using System;
using Avalime.controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalime.UI.controls.IF;
namespace Avalime.UI.controls;

public class SwipeBtnFn{
	public Point _startPoint{get;set;}
	public bool _isSwiping{get;set;}
	public f64 swipeThreshold{get;set;}=50;

	// 定义自定义滑动事件
	//public event EventHandler<SwipeEventArgs>? Swipe;
	public Func<SwipeEventArgs, zero>? onSwipe;

	// public Func<PointerPressedEventArgs, zero> fn_OnPointerPressed(Button z){
	// 	return (PointerPressedEventArgs e)=>{
	// 		_startPoint = e.GetPosition(z); // 记录初始触摸点
	// 		_isSwiping = true;
	// 		e.Handled = true; // 阻止事件冒泡
	// 		return 0;
	// 	};
	// }

	public zero _OnPointerPressed(Button z, PointerPressedEventArgs e){
		_startPoint = e.GetPosition(z); // 记录初始触摸点
		_isSwiping = true;
		e.Handled = true; // 阻止事件冒泡
		return 0;
	}

	public zero _OnPointerReleased(Button z, PointerReleasedEventArgs e){
		if (!_isSwiping){return 0;}
		var currentPoint = e.GetPosition(z);
		var delta = currentPoint - _startPoint;
		// 判断滑动方向（可调整阈值）
		if (Math.Abs(delta.X) > swipeThreshold || Math.Abs(delta.Y) > swipeThreshold) {
			var direction = _getSwipeDirection(delta);
			//Swipe?.Invoke(this, new SwipeEventArgs(direction));//TODO
			onSwipe?.Invoke(new SwipeEventArgs(direction));
			_isSwiping = false; // 触发后结束滑动
		}
		return 0;
	}

	// public Func<PointerEventArgs, zero> fn_OnPointerMoved(Button z){
	// 	return (e)=>{
	// 		if (!_isSwiping){return 0;}
	// 		var currentPoint = e.GetPosition(z);
	// 		var delta = currentPoint - _startPoint;
	// 		// 判断滑动方向（可调整阈值）
	// 		if (Math.Abs(delta.X) > swipeThreshold || Math.Abs(delta.Y) > swipeThreshold) {
	// 			var direction = _getSwipeDirection(delta);
	// 			Swipe?.Invoke(this, new SwipeEventArgs(direction));
	// 			_isSwiping = false; // 触发后结束滑动
	// 		}
	// 		return 0;
	// 	};
	// }

	public static SwipeDirection _getSwipeDirection(Vector delta) {
		if (Math.Abs(delta.X) > Math.Abs(delta.Y)) {
			return delta.X > 0 ? SwipeDirection.Right : SwipeDirection.Left;
		} else {
			return delta.Y > 0 ? SwipeDirection.Down : SwipeDirection.Up;
		}
	}
}

// public class SwipeEventArgs : EventArgs {
// 	public SwipeDirection Direction { get; }

// 	public SwipeEventArgs(SwipeDirection direction) {
// 		Direction = direction;
// 	}
// }


// public enum SwipeDirection {
// 	Up,
// 	Down,
// 	Left,
// 	Right
// }