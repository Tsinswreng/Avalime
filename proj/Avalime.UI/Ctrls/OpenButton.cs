/*

Func<$1, zero>
 */
using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia;
using Avalonia.Controls.Primitives;

namespace Avalime.controls;


public class OpenButton:Button{

	public OpenButton():base(){

	}

	protected override Type StyleKeyOverride => typeof(Button);

	#region override
	public Func<KeyEventArgs, zero>? f_OnKeyDown{get;set;}

	public void b_OnKeyDown(KeyEventArgs e){
		base.OnKeyDown(e);
	}

	protected override void OnKeyDown(KeyEventArgs e){
		if(f_OnKeyDown == null){
			base.OnKeyDown(e);
			return;
		}
		f_OnKeyDown.Invoke(e);
	}
	//-

	public Func<KeyEventArgs, zero>? f_OnKeyUp{get;set;}
	public void b_OnKeyUp(KeyEventArgs e){
		base.OnKeyUp(e);
	}

	protected override void OnKeyUp(KeyEventArgs e){
		if(f_OnKeyUp == null){
			base.OnKeyUp(e);
			return;
		}
		f_OnKeyUp.Invoke(e);
	}
	//-

	public Func<PointerPressedEventArgs, zero>? f_OnPointerPressed{get;set;}
	public void b_OnPointerPressed(PointerPressedEventArgs e){
		base.OnPointerPressed(e);
	}

	protected override void OnPointerPressed(PointerPressedEventArgs e){
		if(f_OnPointerPressed == null){
			base.OnPointerPressed(e);
			return;
		}
		f_OnPointerPressed.Invoke(e);
	}
	//-

	public Func<PointerReleasedEventArgs, zero>? f_OnPointerReleased{get;set;}
	public void b_OnPointerReleased(PointerReleasedEventArgs e){
		base.OnPointerReleased(e);
	}

	protected override void OnPointerReleased(PointerReleasedEventArgs e){
		if(f_OnPointerReleased == null){
			base.OnPointerReleased(e);
			return;
		}
		f_OnPointerReleased.Invoke(e);
	}
	//-

	public Func<PointerCaptureLostEventArgs, zero>? f_OnPointerCaptureLost{get;set;}
	public void b_OnPointerCaptureLost(PointerCaptureLostEventArgs e){
		base.OnPointerCaptureLost(e);
	}

	protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e){
		if(f_OnPointerCaptureLost == null){
			base.OnPointerCaptureLost(e);
			return;
		}
		f_OnPointerCaptureLost.Invoke(e);
	}
	//-

	public Func<RoutedEventArgs, zero>? f_OnLostFocus{get;set;}
	public void b_OnLostFocus(RoutedEventArgs e){
		base.OnLostFocus(e);
	}
	protected override void OnLostFocus(RoutedEventArgs e){
		if(f_OnLostFocus == null){
			base.OnLostFocus(e);
			return;
		}
		f_OnLostFocus.Invoke(e);
	}
	//-
	public Func<TemplateAppliedEventArgs, zero>? f_OnApplyTemplate{get;set;}
	public void b_OnApplyTemplate(TemplateAppliedEventArgs e){
		base.OnApplyTemplate(e);
	}
	protected override void OnApplyTemplate(TemplateAppliedEventArgs e){
		if(f_OnApplyTemplate == null){
			base.OnApplyTemplate(e);
			return;
		}
		f_OnApplyTemplate.Invoke(e);
	}
	//-
	public Func<AvaloniaPropertyChangedEventArgs, zero>? f_OnPropertyChanged{get;set;}
	public void b_OnPropertyChanged(AvaloniaPropertyChangedEventArgs change){
		base.OnPropertyChanged(change);
	}
	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change){
		if(f_OnPropertyChanged == null){
			base.OnPropertyChanged(change);
			return;
		}
		f_OnPropertyChanged.Invoke(change);
	}

	#endregion override

	#region virtualOfParent
	//-
	public Action? f_OnClick{get;set;}
	public void b_OnClick(){
		base.OnClick();
	}

	protected override void OnClick(){
		if(f_OnClick == null){
			base.OnClick();
			return;
		}
		f_OnClick.Invoke();
	}
	#endregion virtual
}