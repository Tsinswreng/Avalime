using System;
using Avalime.controls;
using Avalime.UI.controls.IF;
using Avalonia.Controls;
using Avalonia.Input;

namespace Avalime.UI.controls;

public class LongPressBtn
	: OpenButton
	, I_LongPressBtn
{
	public long longPressDurationMs{
		get{return _longPressBtnFn.longPressDurationMs;}
		set{_longPressBtnFn.longPressDurationMs = value;}
	}
	public event EventHandler? LongPressed;

	public LongPressBtnFn _longPressBtnFn{get;set;} = new LongPressBtnFn();
	public LongPressBtn():base(){
		_longPressBtnFn.init();
		//_longPressBtnFn.onClick = ()=>{OnClick();return 0;};
		_longPressBtnFn.onLongPress = ()=>{
			LongPressed?.Invoke(this, EventArgs.Empty);
			return 0;
		};
	}

	protected override void OnPointerPressed(PointerPressedEventArgs e){
		base.OnPointerPressed(e);
		_longPressBtnFn._OnPointerPressed(e);
	}

	protected override void OnPointerReleased(PointerReleasedEventArgs e){
		base.OnPointerReleased(e);
		_longPressBtnFn._OnPointerReleased(e);
	}

	protected override void OnClick(){
		//System.Console.WriteLine("override OnClick");
		if(!_longPressBtnFn._OnClick()){
			return;
		}
		base.OnClick();
	}

}
