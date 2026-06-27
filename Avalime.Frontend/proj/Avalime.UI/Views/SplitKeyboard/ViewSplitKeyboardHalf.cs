using Avalonia.Controls;
using Avalonia.Layout;
using Avalime.UI.Views.KeyBoard;

namespace Avalime.UI.Views.SplitKeyboard;

/// <summary>
/// 分體懸浮窗中的單側鍵盤。
/// 這裡不重新實現半套鍵盤，而是放入一個“兩倍寬”的完整鍵盤，
/// 再用容器裁剪只露出左半或右半。
/// 這樣現有按鍵、長按、滑動與數字層邏輯都可直接復用。
/// </summary>
public class ViewSplitKeyboardHalf : UserControl
	, IDisposable
{
	readonly ViewKeyBoard _keyboard;
	readonly SplitKeyboardSide _side;
	readonly Grid _root = new();

	public ViewSplitKeyboardHalf(SplitKeyboardSide Side){
		_side = Side;
		_keyboard = new ViewKeyBoard();
		Render();
	}

	void Render()
	{
		ClipToBounds = true;
		_root.ClipToBounds = true;
		Content = _root;
		_root.Children.Add(_keyboard);
		LayoutUpdated += OnLayoutUpdated;
	}

	/// <summary>
	/// overlay 的寬度是目標半邊寬度，而內部完整鍵盤需要拉成兩倍寬。
	/// 左側貼左對齊即可露出前半；右側貼右對齊即可露出後半。
	/// </summary>
	void OnLayoutUpdated(object? Sender, EventArgs E)
	{
		var width = Bounds.Width;
		if(width <= 0){
			return;
		}
		_keyboard.Width = width * 2;
		_keyboard.HorizontalAlignment = _side == SplitKeyboardSide.Left
			? HorizontalAlignment.Left
			: HorizontalAlignment.Right;
		_keyboard.VerticalAlignment = VerticalAlignment.Stretch;
	}

	public void Dispose()
	{
		LayoutUpdated -= OnLayoutUpdated;
		_keyboard.Dispose();
	}
}
