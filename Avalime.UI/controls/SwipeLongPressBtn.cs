
using System;
using Avalime.controls;
using Avalime.UI.controls.IF;
using Avalonia.Input;

namespace Avalime.UI.controls;

public class SwipeLongPressBtn
	:OpenButton
	,I_SwipeBtn
	,I_LongPressBtn
{

	public f64 SwipeThreshold{
		get{return _swipeBtnFn.swipeThreshold;}
		set{_swipeBtnFn.swipeThreshold=value;}
	}

	public event EventHandler<IF.SwipeEventArgs>? Swipe;
	public SwipeBtnFn _swipeBtnFn{get;set;}=new SwipeBtnFn();


	public long longPressDurationMs{
		get{return _longPressBtnFn.longPressDurationMs;}
		set{_longPressBtnFn.longPressDurationMs = value;}
	}
	public event EventHandler? LongPressed;

	public LongPressBtnFn _longPressBtnFn{get;set;} = new LongPressBtnFn();


	public SwipeLongPressBtn():base(){
		_swipeBtnFn.onSwipe = (e)=>{
			Swipe?.Invoke(this,e);
			return 0;
		};
		_longPressBtnFn.init();
		//_longPressBtnFn.onClick = ()=>{OnClick();return 0;};
		_longPressBtnFn.onLongPress = ()=>{
			LongPressed?.Invoke(this, EventArgs.Empty);
			return 0;
		};
	}

	protected override void OnPointerPressed(PointerPressedEventArgs e) {
		base.OnPointerPressed(e);
		_swipeBtnFn._OnPointerPressed(this,e);
		_longPressBtnFn._OnPointerPressed(e);
	}

	protected override void OnPointerReleased(PointerReleasedEventArgs e) {
		base.OnPointerReleased(e);
		_swipeBtnFn._OnPointerReleased(this,e);
		_longPressBtnFn._OnPointerReleased(e);
	}


	protected override void OnClick(){
		if(!_longPressBtnFn._OnClick()){
			return;
		}
		base.OnClick();
	}

}