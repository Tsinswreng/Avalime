using System;
using Avalime.controls;
using Avalime.UI.controls.IF;
using Avalonia.Input;

namespace Avalime.UI.controls;

public class SwipeBtn
	: OpenButton
	, I_SwipeBtn
{
	public f64 SwipeThreshold{
		get{return _swipeBtnFn.swipeThreshold;}
		set{_swipeBtnFn.swipeThreshold=value;}
	}

	public event EventHandler<IF.SwipeEventArgs>? Swipe;
	public SwipeBtnFn _swipeBtnFn{get;set;}=new SwipeBtnFn();

	public SwipeBtn():base(){
		//_swipeBtnFn.init();
		_swipeBtnFn.onSwipe = (e)=>{
			Swipe?.Invoke(this,e);
			return 0;
		};
	}

	protected override void OnPointerPressed(PointerPressedEventArgs e) {
		base.OnPointerPressed(e);
		_swipeBtnFn._OnPointerPressed(this,e);
	}

	protected override void OnPointerReleased(PointerReleasedEventArgs e) {
		base.OnPointerReleased(e);
		_swipeBtnFn._OnPointerReleased(this,e);
	}


}