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
	static class TswG{
		public static readonly SolidColorBrush KeyBg = Brush("#000000");
		public static readonly SolidColorBrush KeyText = Brush("#E0E0E0");
		public static readonly SolidColorBrush KeyBorder = Brush("#ECEFF1");
		public static readonly SolidColorBrush HintText = Brush("#BDBDBD");
		public static readonly SolidColorBrush PressedBg = Brush("#4DB6AC");
		public static readonly SolidColorBrush PressedText = Brush("#37474F");
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
			.Set(BorderBrushProperty, TswG.KeyBorder)
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
			.Set(VerticalAlignmentProperty, VAlign.Top)
			.Set(MarginProperty, new Thickness(0, 2, 0, 0))
		);
	}

	#region 手勢（用Border直控、不用Button避免事件被吞）
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
			//判定滑動方向
			if(Math.Abs(dx) > Math.Abs(dy)){
				if(dx > 0) Ctx?.SwipeRight?.Invoke();
				else Ctx?.SwipeLeft?.Invoke();
			}else{
				if(dy > 0) Ctx?.SwipeDown?.Invoke();
				else Ctx?.SwipeUP?.Invoke();
			}
		}else{
			//普通點擊
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

		//在Border上直接處理所有手勢、不用Button
		border.PointerPressed += OnPointerPressed;
		border.PointerMoved += OnPointerMoved;
		border.PointerReleased += OnPointerReleased;
		border.PointerCaptureLost += OnPointerCaptureLost;

		border.SetChild(new Grid(), grid=>{
			//Hint: Auto高度、Label: * 填充剩餘
			grid.RowDefinitions = new("Auto,*");
			grid.A(new TextBlock(), tb=>{
				tb.Classes.Add(Cls.HintLabel);
				Ctx.Bind(tb, x=>x.Text, x=>x.Hint);
				Grid.SetRow(tb, 0);
			}).A(new TextBlock(), tb=>{
				tb.Classes.Add(Cls.Label);
				Ctx.Bind(tb, x=>x.Text, x=>x.Label);
				Grid.SetRow(tb, 1);
			});
		});
	}
}
