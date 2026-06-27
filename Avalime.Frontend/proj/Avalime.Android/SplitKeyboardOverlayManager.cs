using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using Avalime.Core.Infra.Log;
using Avalime.UI;
using Avalime.UI.Views.SplitKeyboard;

namespace Avalime.Android;

/// <summary>
/// 管理 Android 端分體鍵盤的三個 overlay window：
/// 左半鍵盤、右半鍵盤，以及中間窄頂條。
/// IME 輸入鏈仍由 InputMethodService 持有；overlay 只承擔顯示與觸摸輸入。
/// </summary>
public class SplitKeyboardOverlayManager
	: IDisposable
{
	readonly Context _context;
	readonly IViewManager? _viewManager;
	LoggingAvaloniaView? _leftView;
	LoggingAvaloniaView? _rightView;
	LoggingAvaloniaView? _centerView;
	bool _isShown;

	public SplitKeyboardOverlayManager(Context Context){
		_context = Context;
		_viewManager = Context.GetSystemService(Context.WindowService)?.JavaCast<IViewManager>();
	}

	public bool Show()
	{
		if(_viewManager is null){
			AppLog.Warn("[SplitOverlay] ViewManager unavailable");
			return false;
		}
		if(!AvalimeOverlayPermission.CanDraw(_context)){
			AppLog.Warn("[SplitOverlay] missing overlay permission");
			return false;
		}
		if(_isShown){
			UpdateLayout();
			return true;
		}

		try{
			_leftView = CreateKeyboardOverlayView(SplitKeyboardSide.Left);
			_rightView = CreateKeyboardOverlayView(SplitKeyboardSide.Right);
			_centerView = CreateCenterOverlayView();

			_viewManager.AddView(_leftView, BuildSideLayoutParams(isLeft: true));
			_viewManager.AddView(_rightView, BuildSideLayoutParams(isLeft: false));
			_viewManager.AddView(_centerView, BuildCenterLayoutParams());
			_isShown = true;
			AppLog.Info("[SplitOverlay] shown");
			return true;
		}catch(Exception Ex){
			AppLog.Error(Ex, "[SplitOverlay] Show failed");
			Hide();
			return false;
		}
	}

	public void UpdateLayout()
	{
		if(!_isShown || _viewManager is null){
			return;
		}
		try{
			if(_leftView is not null){
				_viewManager.UpdateViewLayout(_leftView, BuildSideLayoutParams(isLeft: true));
			}
			if(_rightView is not null){
				_viewManager.UpdateViewLayout(_rightView, BuildSideLayoutParams(isLeft: false));
			}
			if(_centerView is not null){
				_viewManager.UpdateViewLayout(_centerView, BuildCenterLayoutParams());
			}
		}catch(Exception Ex){
			AppLog.Error(Ex, "[SplitOverlay] UpdateLayout failed");
		}
	}

	public void Hide()
	{
		if(_viewManager is not null){
			RemoveOverlayView(_leftView);
			RemoveOverlayView(_rightView);
			RemoveOverlayView(_centerView);
		}
		DisposeOverlayView(ref _leftView);
		DisposeOverlayView(ref _rightView);
		DisposeOverlayView(ref _centerView);
		_isShown = false;
	}

	LoggingAvaloniaView CreateKeyboardOverlayView(SplitKeyboardSide Side)
	{
		var view = new LoggingAvaloniaView(_context);
		view.Content = new ViewSplitKeyboardHalf(Side);
		return view;
	}

	LoggingAvaloniaView CreateCenterOverlayView()
	{
		var view = new LoggingAvaloniaView(_context);
		view.Content = new ViewSplitTopOverlay();
		return view;
	}

	WindowManagerLayoutParams BuildSideLayoutParams(bool isLeft)
	{
		var metrics = _context.Resources?.DisplayMetrics;
		var screenWidth = metrics?.WidthPixels ?? 0;
		var screenHeight = metrics?.HeightPixels ?? 0;
		var width = Math.Max(1, screenWidth / 4);
		var height = Math.Max(1, (i32)(screenHeight * 0.8));
		var gravity = isLeft
			? GravityFlags.Left | GravityFlags.Bottom
			: GravityFlags.Right | GravityFlags.Bottom;
		return new WindowManagerLayoutParams(
			width,
			height,
			GetOverlayWindowType(),
			GetCommonFlags(),
			Format.Translucent
		){
			Gravity = gravity,
			X = 0,
			Y = 0
		};
	}

	WindowManagerLayoutParams BuildCenterLayoutParams()
	{
		var metrics = _context.Resources?.DisplayMetrics;
		var screenWidth = metrics?.WidthPixels ?? 0;
		var screenHeight = metrics?.HeightPixels ?? 0;
		var keyboardHeight = Math.Max(1, (i32)(screenHeight * 0.8));
		var width = Math.Max(1, screenWidth / 2);
		return new WindowManagerLayoutParams(
			width,
			ViewGroup.LayoutParams.WrapContent,
			GetOverlayWindowType(),
			GetCommonFlags(),
			Format.Translucent
		){
			Gravity = GravityFlags.CenterHorizontal | GravityFlags.Bottom,
			X = 0,
			Y = keyboardHeight
		};
	}

	static WindowManagerTypes GetOverlayWindowType()
	{
		if(global::Android.OS.Build.VERSION.SdkInt >= global::Android.OS.BuildVersionCodes.O){
			return WindowManagerTypes.ApplicationOverlay;
		}
		return WindowManagerTypes.Phone;
	}

	static WindowManagerFlags GetCommonFlags()
	{
		return WindowManagerFlags.NotFocusable
			| WindowManagerFlags.NotTouchModal
			| WindowManagerFlags.LayoutInScreen;
	}

	void RemoveOverlayView(View? View)
	{
		if(_viewManager is null || View is null){
			return;
		}
		try{
			_viewManager.RemoveView(View);
		}catch(Exception Ex){
			AppLog.Error(Ex, "[SplitOverlay] RemoveView failed");
		}
	}

	static void DisposeOverlayView(ref LoggingAvaloniaView? View)
	{
		if(View?.Content is IDisposable disposableContent){
			disposableContent.Dispose();
		}
		if(View is not null){
			View.Content = null;
			View.Dispose();
			View = null;
		}
	}

	public void Dispose()
	{
		Hide();
	}
}
