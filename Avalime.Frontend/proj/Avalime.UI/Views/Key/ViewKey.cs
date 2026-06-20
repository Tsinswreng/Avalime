//ViewKey: 單個鍵盤按鍵視圖、支援點擊/長按/滑動 + Hint提示文字、樣式匹配 TswG 暗色方案
using Avalime.Core.Infra;
using Avalime.Core.Infra.Log;
using Avalime.ViewModels.key;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalime.UI.Infra;

namespace Avalime.UI.Views.Key;
using Ctx = KeyVm;

public class ViewKey : AppViewBase<Ctx>
{
	public ViewKey(){
		Ctx = new Ctx(Di.GetRSvc<RimeConnectionState>());
		Style();
		Render();
	}

	public class Cls{
		public const str HintLabel = nameof(HintLabel);
		public const str Label = nameof(Label);
		public const str KeyBorder = nameof(KeyBorder);
	}

	/// TswG 暗色配色
	/// 分隔線顏色 = 鍵盤底色、模擬TswG的gap效果、相鄰兩鍵0.5+0.5=1px線即背景色
	static class TswG{
		public static readonly SolidColorBrush KeyText = Brush("#E0E0E0");
		public static readonly SolidColorBrush GapLine = Brush("#253238"); //邊框線=鍵盤底色(keyboard_back_color)
		public static readonly SolidColorBrush HintText = Brush("#BDBDBD");
		public static readonly CornerRadius Round = new(0);
		public static readonly Thickness KeyMargin = new(0);
		public static readonly Thickness BorderThick = new(0.5);
	}

	const double SwipeThreshold = 20;
	const int LongPressMs = 400;
	const int RepeatIntervalMs = 50;

	static SolidColorBrush Brush(str Hex)=>SolidColorBrush.Parse(Hex);

	void Style(){
		Styles.A(
			Sty.Is<Control>()
			.Set(CornerRadiusProperty, TswG.Round)
		).A(
			Sty.Is<Border>(x=>x.Class(Cls.KeyBorder))
			.Set(MarginProperty, TswG.KeyMargin)
			.Set(PaddingProperty, new Thickness(0))
			.Set(BackgroundProperty, UiCfg.Inst.KeyBgColor)
			.Set(BorderBrushProperty, TswG.GapLine)       //邊框=鍵盤底色、模擬gap
			.Set(BorderThicknessProperty, TswG.BorderThick)
		).A(
			Sty.Is<Control>(x=>x.Class(Cls.Label))
			.Set(MinHeightProperty, 0.0)
			.Set(MinWidthProperty, 0.0)
			.Set(HorizontalAlignmentProperty, HAlign.Center)
			.Set(VerticalAlignmentProperty, VAlign.Center)
			.Set(ForegroundProperty, TswG.KeyText)
		).A(
			Sty.Is<Control>(x=>x.Class(Cls.HintLabel))
			.Set(FontSizeProperty, UiCfg.Inst.HintFontSize)
			.Set(ForegroundProperty, TswG.HintText)
			.Set(HorizontalAlignmentProperty, HAlign.Center)
		);
	}

	#region 手勢
	Border _border = default!;
	Point _pressPoint;
	DispatcherTimer? _longPressTimer;
	DispatcherTimer? _repeatTimer;
	bool _longPressFired;

	void OnPointerPressed(object? s, PointerPressedEventArgs e){
		_pressPoint = e.GetPosition(_border);
		_longPressFired = false;
		e.Pointer.Capture(_border);
		_border.Background = UiCfg.Inst.MainColor; //按下視覺反饋
		AppLog.Debug($"[Key] Pressed, hasLongPress={Ctx?.LongPress is not null}, isRepeat={Ctx?.IsRepeat}");
		StartLongPressTimer();
	}

	void OnPointerMoved(object? s, PointerEventArgs e){
		var pos = e.GetPosition(_border);
		var dx = pos.X - _pressPoint.X;
		var dy = pos.Y - _pressPoint.Y;
		if(Math.Abs(dx) > SwipeThreshold || Math.Abs(dy) > SwipeThreshold)
			StopLongPressTimer();
	}

	void OnPointerReleased(object? s, PointerReleasedEventArgs e){
		var swPerf = System.Diagnostics.Stopwatch.StartNew();
		if(e.Pointer.Captured == _border){
			e.Pointer.Capture(null);
		}
		StopLongPressTimer();
		RestoreBg();

		if(_longPressFired){
			AppLog.Debug("[Key] Released after long press, skipping Click");
			return;
		}
		var pos = e.GetPosition(_border);
		var dx = pos.X - _pressPoint.X;
		var dy = pos.Y - _pressPoint.Y;

		if(Math.Abs(dx) > SwipeThreshold || Math.Abs(dy) > SwipeThreshold){
			if(Math.Abs(dx) > Math.Abs(dy)){
				if(dx > 0) Ctx?.SwipeRight?.Invoke();
				else Ctx?.SwipeLeft?.Invoke();
			}else{
				if(dy > 0) Ctx?.SwipeDown?.Invoke();
				else Ctx?.SwipeUP?.Invoke();
			}
		}else{
			AppLog.Debug($"[Perf] OnPointerReleased→Click start: {swPerf.ElapsedMilliseconds}ms");
			Ctx?.Click?.Invoke();
			AppLog.Debug($"[Perf] OnPointerReleased→Click done: {swPerf.ElapsedMilliseconds}ms");
		}
	}

	void OnPointerCaptureLost(object? s, PointerCaptureLostEventArgs e){
		StopLongPressTimer();
		RestoreBg();
	}

	void RestoreBg(){
		_border.Background = Ctx?.Background ?? UiCfg.Inst.KeyBgColor;
	}

	void StartLongPressTimer(){
		_longPressTimer = new DispatcherTimer{Interval = TimeSpan.FromMilliseconds(LongPressMs)};
		_longPressTimer.Tick += (_, _) => {
			_longPressTimer.Stop();
			_longPressFired = true;
			AppLog.Debug($"[Key] LongPress fired, isRepeat={Ctx?.IsRepeat}");
			Ctx?.LongPress?.Invoke();
			if(Ctx?.IsRepeat == true){
				StartRepeatTimer();
			}
		};
		_longPressTimer.Start();
	}

	void StartRepeatTimer(){
		_repeatTimer = new DispatcherTimer{Interval = TimeSpan.FromMilliseconds(RepeatIntervalMs)};
		_repeatTimer.Tick += (_, _) => {
			Ctx?.Click?.Invoke();
		};
		_repeatTimer.Start();
	}

	void StopLongPressTimer(){
		_longPressTimer?.Stop();
		_longPressTimer = null;
		_repeatTimer?.Stop();
		_repeatTimer = null;
	}
	#endregion

	void Render(){
		var keyboardFont = UiCfg.Inst.KeyboardFontFamily;
		var border = new Border();
		_border = border;
		border.Classes.Add(Cls.KeyBorder);
		Content = border;

		border.PointerPressed += OnPointerPressed;
		border.PointerMoved += OnPointerMoved;
		border.PointerReleased += OnPointerReleased;
		border.PointerCaptureLost += OnPointerCaptureLost;
		Ctx.Bind(border, Border.BackgroundProperty, x=>x.Background);

		border.SetChild(new Grid(), grid=>{
			//單格疊放：Hint疊在頂部、Label居中、BottomHint在底部、互不搶空間
			var label = new TextBlock();
			label.Classes.Add(Cls.Label);
			if(keyboardFont is not null) label.FontFamily = keyboardFont;
			Ctx.Bind(label, x=>x.Text, x=>x.Label);
			Ctx.Bind(label, TextBlock.FontSizeProperty, x=>x.FontSize);

			var hint = new TextBlock();
			hint.Classes.Add(Cls.HintLabel);
			if(keyboardFont is not null) hint.FontFamily = keyboardFont;
			hint.VerticalAlignment = VAlign.Top;
			hint.HorizontalAlignment = HAlign.Right;
			hint.Margin = new(0, 1, 3, 0);
			Ctx.Bind(hint, x=>x.Text, x=>x.Hint);

			var hintBottom = new TextBlock();
			hintBottom.Classes.Add(Cls.HintLabel);
			if(keyboardFont is not null) hintBottom.FontFamily = keyboardFont;
			hintBottom.VerticalAlignment = VAlign.Bottom;
			hintBottom.HorizontalAlignment = HAlign.Left;
			hintBottom.Margin = new(2, 0, 0, 2);
			Ctx.Bind(hintBottom, x=>x.Text, x=>x.BottomHint);

			grid.Children.Add(label);
			grid.Children.Add(hint);       //hint在右上角
			grid.Children.Add(hintBottom); //hintBottom在左下角
		});
	}
}
