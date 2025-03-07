using System;
using Avalonia.Input;
using Avalonia.Threading;

namespace Avalime.UI.controls;

public class LongPressBtnFn{

	protected DispatcherTimer _pressTimer;
	protected bool _isLongPressTriggered;

	protected bool _hasLongPressed = false;
	public i64 longPressDurationMs{get;set;} = 500; // in milliseconds
	//public Func<zero> onClick{get;set;} = ()=>0; // 点击事件
	public Func<zero> onLongPress{get;set;} = ()=>0; // 长按事件

	public zero init(){
		_pressTimer = new DispatcherTimer(){
			Interval = TimeSpan.FromMilliseconds(longPressDurationMs)
		};
		_pressTimer.Tick += OnTimerElapsed;
		return 0;
	}

	public zero _OnPointerPressed(PointerPressedEventArgs e){
		_isLongPressTriggered = false;
		_pressTimer.Start();
		return 0;
	}

	public zero _OnPointerReleased(PointerReleasedEventArgs e){
		_pressTimer.Stop(); // 松开时停止计时器
		if (!_isLongPressTriggered) {
			// 未触发长按时，执行点击逻辑
			//OnClick();
			//onClick?.Invoke();
		}
		_isLongPressTriggered = false;
		return 0;
	}

	private void OnTimerElapsed(object? sender, EventArgs e){
		_isLongPressTriggered = true;
		_pressTimer.Stop();
		// 触发长按事件
		onLongPress?.Invoke();
		_hasLongPressed = true;
	}


	public bool _OnClick(){
		if (_hasLongPressed) {
			_hasLongPressed = false;
			return false;
		}
		return true;
	}

	// public event EventHandler? LongPressed;
	// public virtual void OnLongPress(){
	// 	LongPressed?.Invoke(this, EventArgs.Empty);
	// }

}

