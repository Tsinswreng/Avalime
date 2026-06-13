//ViewKey: 單個鍵盤按鍵視圖、支援點擊/長按/滑動 + Hint提示文字、樣式匹配 TswG 暗色方案
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
		Ctx = Ctx.Mk();
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
		public static readonly SolidColorBrush KeyBg = Brush("#000000");
		public static readonly SolidColorBrush KeyText = Brush("#E0E0E0");
		public static readonly SolidColorBrush GapLine = Brush("#253238"); //邊框線=鍵盤底色(keyboard_back_color)
		public static readonly SolidColorBrush HintText = Brush("#BDBDBD");
		public static readonly SolidColorBrush PressedBg = Brush("#4DB6AC");
		public static readonly CornerRadius Round = new(0);
		public static readonly Thickness KeyMargin = new(0);
		public static readonly Thickness BorderThick = new(0.5);
	}

	const double SwipeThreshold = 20;
	const int LongPressMs = 400;

	static SolidColorBrush Brush(str Hex)=>SolidColorBrush.Parse(Hex);

	void Style(){
		Styles.A(
			Sty.Is<Control>()
			.Set(CornerRadiusProperty, TswG.Round)
		).A(
			Sty.Is<Border>(x=>x.Class(Cls.KeyBorder))
			.Set(MarginProperty, TswG.KeyMargin)
			.Set(PaddingProperty, new Thickness(0))
			.Set(BackgroundProperty, TswG.KeyBg)
			.Set(BorderBrushProperty, TswG.GapLine)       //邊框=鍵盤底色、模擬gap
			.Set(BorderThicknessProperty, TswG.BorderThick)
		).A(
			Sty.Is<Control>(x=>x.Class(Cls.Label))
			.Set(MinHeightProperty, 0.0)
			.Set(MinWidthProperty, 0.0)
			.Set(HorizontalAlignmentProperty, HAlign.Center)
			.Set(VerticalAlignmentProperty, VAlign.Center)
			.Set(FontSizeProperty, 24.0)
			.Set(ForegroundProperty, TswG.KeyText)
		).A(
			Sty.Is<Control>(x=>x.Class(Cls.HintLabel))
			.Set(FontSizeProperty, 10.0)
			.Set(ForegroundProperty, TswG.HintText)
			.Set(HorizontalAlignmentProperty, HAlign.Center)
		);
	}

	#region 手勢
	Border _border = default!;
	Point _pressPoint;
	DispatcherTimer? _longPressTimer;
	bool _longPressFired;

	void OnPointerPressed(object? s, PointerPressedEventArgs e){
		_pressPoint = e.GetPosition(_border);
		_longPressFired = false;
		_border.Background = TswG.PressedBg; //按下視覺反饋
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
		StopLongPressTimer();
		_border.Background = TswG.KeyBg; //復原背景

		if(_longPressFired) return;
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
			Ctx?.Click?.Invoke();
		}
	}

	void OnPointerCaptureLost(object? s, PointerCaptureLostEventArgs e){
		StopLongPressTimer();
		_border.Background = TswG.KeyBg;
	}

	void StartLongPressTimer(){
		_longPressTimer = new DispatcherTimer{Interval = TimeSpan.FromMilliseconds(LongPressMs)};
		_longPressTimer.Tick += (_, _) => {
			_longPressTimer.Stop();
			_longPressFired = true;
			Ctx?.LongPress?.Invoke();
		};
		_longPressTimer.Start();
	}

	void StopLongPressTimer(){
		_longPressTimer?.Stop();
		_longPressTimer = null;
	}
	#endregion

	void Render(){
		var border = new Border();
		_border = border;
		border.Classes.Add(Cls.KeyBorder);
		Content = border;

		border.PointerPressed += OnPointerPressed;
		border.PointerMoved += OnPointerMoved;
		border.PointerReleased += OnPointerReleased;
		border.PointerCaptureLost += OnPointerCaptureLost;

		border.SetChild(new Grid(), grid=>{
			//單格疊放：Hint疊在頂部、Label居中、互不搶空間
			var label = new TextBlock();
			label.Classes.Add(Cls.Label);
			Ctx.Bind(label, x=>x.Text, x=>x.Label);

			var hint = new TextBlock();
			hint.Classes.Add(Cls.HintLabel);
			hint.VerticalAlignment = VAlign.Top;
			hint.Margin = new(0, 1, 0, 0);
			Ctx.Bind(hint, x=>x.Text, x=>x.Hint);

			grid.Children.Add(label);
			grid.Children.Add(hint); //hint在label上層
		});
	}
}
