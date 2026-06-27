using Avalonia.Controls;
using Avalonia.Layout;

namespace Avalime.UI.Views.SplitKeyboard;

/// <summary>
/// 分體模式下的單側頂條。
/// 與鍵盤半邊相同，復用一個“兩倍寬”的完整頂條，再裁剪出左半或右半。
/// 這樣候選欄、工具欄、預編輯欄都沿同一條全寬佈局切開，中央保持空出。
/// </summary>
public class ViewSplitTopHalf : UserControl
	, IDisposable
{
	readonly ViewSplitTopOverlay _fullStrip;
	readonly SplitKeyboardSide _side;
	readonly Grid _root = new();

	public ViewSplitTopHalf(SplitKeyboardSide Side){
		_side = Side;
		_fullStrip = new ViewSplitTopOverlay();
		Render();
	}

	void Render()
	{
		ClipToBounds = true;
		_root.ClipToBounds = true;
		Content = _root;
		_root.Children.Add(_fullStrip);
		LayoutUpdated += OnLayoutUpdated;
	}

	void OnLayoutUpdated(object? Sender, EventArgs E)
	{
		var width = Bounds.Width;
		var height = Bounds.Height;
		if(width <= 0){
			return;
		}
		_fullStrip.Width = width * 2;
		if(height > 0){
			_fullStrip.Height = height;
		}
		_fullStrip.HorizontalAlignment = _side == SplitKeyboardSide.Left
			? HorizontalAlignment.Left
			: HorizontalAlignment.Right;
		_fullStrip.VerticalAlignment = VerticalAlignment.Stretch;
	}

	public void Dispose()
	{
		LayoutUpdated -= OnLayoutUpdated;
		_fullStrip.Dispose();
	}
}
