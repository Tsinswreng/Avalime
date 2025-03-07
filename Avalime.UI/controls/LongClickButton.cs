using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia;

namespace Avalime.controls;

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using System;

public class LongPressButton : Button {
	private DispatcherTimer _pressTimer;
	private bool _isLongPressTriggered;

	protected bool _hasLongPressed = false;

	public LongPressButton() {
		// 初始化计时器，设置长按阈值（例如 500 毫秒）
		_pressTimer = new DispatcherTimer {
			Interval = TimeSpan.FromMilliseconds(500)
		};
		_pressTimer.Tick += OnTimerElapsed;

		// 订阅指针事件
		this.PointerPressed += _onPointerPressed;
		this.PointerReleased += _onPointerReleased;
	}


	protected override void OnPointerPressed(PointerPressedEventArgs e) {
		base.OnPointerPressed(e);
		//System.Console.WriteLine("override pressed");//t +
		_onPointerPressed(this, e);
	}

	private void _onPointerPressed(object sender, PointerPressedEventArgs e) {
		_isLongPressTriggered = false;
		_pressTimer.Start(); // 按下时启动计时器
	}

	private void _onPointerReleased(object sender, PointerReleasedEventArgs e) {
		_pressTimer.Stop(); // 松开时停止计时器
		if (!_isLongPressTriggered) {
			// 未触发长按时，执行点击逻辑
			OnClick();
		}
		_isLongPressTriggered = false;
	}

	private void OnTimerElapsed(object sender, EventArgs e) {
		_isLongPressTriggered = true;
		_pressTimer.Stop();
		// 触发长按事件
		OnLongPress();
		_hasLongPressed = true;
	}


	protected override void OnClick() {
		if (_hasLongPressed) {
			_hasLongPressed = false;
			return;
		}
		base.OnClick();
	}

	// 自定义长按事件
	public event EventHandler LongPressed;
	protected virtual void OnLongPress() {
		LongPressed?.Invoke(this, EventArgs.Empty);
	}
}
