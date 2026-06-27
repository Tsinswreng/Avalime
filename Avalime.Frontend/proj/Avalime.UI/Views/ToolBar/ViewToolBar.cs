using Avalime.Core.Infra;
using Avalime.UI.Infra;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using System.ComponentModel;
using Avalime.Core.Infra.Log;

namespace Avalime.UI.Views.ToolBar;
using Ctx = VmToolBar;

public class ViewToolBar : AppViewBase<Ctx>
	, IDisposable
{
	GridStack Root = new(IsRow: false);
	readonly ImeUiState _uiState;
	Border? _splitButton;
	Rect _lastLoggedRootBounds;
	Rect _lastLoggedSplitBounds;
	PropertyChangedEventHandler? _uiStatePropertyChangedHandler;

	public ViewToolBar(){
		Ctx = Di.DiOrMk<Ctx>();
		_uiState = Di.DiOrMk<ImeUiState>();
		Render();
	}

	void Render(){
		this.SetContent(Root.Grid);
		Root.Grid.Background = Brushes.Black;
		Root.Grid.Height = GetTopBarHeight();
		Root.Grid.ClipToBounds = true;
		ClipToBounds = true;
		Root.SetColDefs([
			new(1, GUT.Star),
			new(1, GUT.Star),
			new(1, GUT.Star),
			new(1, GUT.Star),
			new(1, GUT.Star),
			new(1, GUT.Star),
			new(1, GUT.Star),
			new(1, GUT.Star),
			new(1, GUT.Star),
			new(1, GUT.Star),
			new(1, GUT.Star),
		]);

		Root
		.A(MkBtn(), b=>{
			b.PointerPressed += (_, e) => {
				e.Handled = true;
				Ctx?.ToggleSimplification();
			};
			b.SetChild(new TextBlock(), o=>{
				o.Foreground = Brushes.White;
				o.VerticalAlignment = VAlign.Center;
				o.HorizontalAlignment = HAlign.Center;
				o.FontSize = UiCfg.Inst.TopBarFontSize;
				Ctx.Bind(o, TextBlock.TextProperty, x => x.HanLabel);
			});
		})
		.A(MkBtn(), b=>{
			b.PointerPressed += (_, e) => {
				e.Handled = true;
				Ctx?.ToggleCandidateComment();
			};
			b.SetChild(new TextBlock(), o=>{
				o.Foreground = Brushes.White;
				o.VerticalAlignment = VAlign.Center;
				o.HorizontalAlignment = HAlign.Center;
				o.FontSize = UiCfg.Inst.TopBarFontSize;
				o.Text = "註";
				Ctx.Bind(o, TextBlock.ForegroundProperty, x => x.CandidateCommentForeground);
			});
		})
		.A(MkBtn(), b=>{
			b.PointerPressed += (_, e) => {
				e.Handled = true;
				Ctx?.ToggleClipboard();
			};
			b.SetChild(Avalime.UI.Icons.Icons.Clipboard(), o=>{
				o.Width = UiCfg.Inst.TopBarFontSize;
				o.Height = UiCfg.Inst.TopBarFontSize;
			});
		})
		.A(MkBtn(), b=>{
			b.PointerPressed += (_, e) => {
				e.Handled = true;
				Ctx?.ToggleRimeLog();
			};
			b.SetChild(Avalime.UI.Icons.Icons.ScrollText(), o=>{
				o.Width = UiCfg.Inst.TopBarFontSize;
				o.Height = UiCfg.Inst.TopBarFontSize;
			});
		})
		.A(MkBtn(), b=>{
			_splitButton = b;
			b.PointerPressed += (_, e) => {
				var localPos = e.GetPosition(b);
				var rootPos = e.GetPosition(Root.Grid);
				var inside = localPos.X >= 0
					&& localPos.Y >= 0
					&& localPos.X <= b.Bounds.Width
					&& localPos.Y <= b.Bounds.Height;
				AppLog.Info($"[SplitTouch] Toolbar Split button PointerPressed local={localPos} root={rootPos} btnBounds={b.Bounds} rootBounds={Root.Grid.Bounds} inside={inside}");
				if(!inside){
					e.Handled = true;
					return;
				}
				e.Handled = true;
				Ctx?.ToggleSplitKeyboard();
			};
			b.SetChild(new TextBlock(), o=>{
				o.IsHitTestVisible = false;
				o.Foreground = Brushes.White;
				o.VerticalAlignment = VAlign.Center;
				o.HorizontalAlignment = HAlign.Center;
				o.FontSize = UiCfg.Inst.TopBarFontSize;
				o.Text = "分";
				Ctx.Bind(o, TextBlock.ForegroundProperty, x => x.SplitKeyboardForeground);
			});
		})
		.A(MkBtn(), b=>{
			b.PointerPressed += (_, e) => {
				e.Handled = true;
				Ctx?.ToggleSystemKeyRemapping();
			};
			b.SetChild(new TextBlock(), o=>{
				o.Foreground = Brushes.White;
				o.VerticalAlignment = VAlign.Center;
				o.HorizontalAlignment = HAlign.Center;
				o.FontSize = UiCfg.Inst.TopBarFontSize;
				o.Text = "映";
				Ctx.Bind(o, TextBlock.ForegroundProperty, x => x.SystemKeyRemappingForeground);
			});
		})
		;
		LayoutUpdated += OnLayoutUpdated;
		_uiStatePropertyChangedHandler = (_, e) => {
			if(e.PropertyName == nameof(ImeUiState.IsCandidateCommentVisible)){
				SyncHeight();
			}
		};
		_uiState.PropertyChanged += _uiStatePropertyChangedHandler;
	}

	double GetTopBarHeight() => _uiState.IsCandidateCommentVisible
		? UiCfg.Inst.TopBarHeight
		: UiCfg.Inst.TopBarHeightNoComment;

	void SyncHeight()
	{
		var height = GetTopBarHeight();
		Root.Grid.Height = height;
		this.Height = height;
	}

	void OnLayoutUpdated(object? Sender, EventArgs E)
	{
		var rootBounds = Root.Grid.Bounds;
		var splitBounds = _splitButton?.Bounds ?? default;
		if(rootBounds == _lastLoggedRootBounds && splitBounds == _lastLoggedSplitBounds){
			return;
		}
		_lastLoggedRootBounds = rootBounds;
		_lastLoggedSplitBounds = splitBounds;
		AppLog.Info($"[SplitLayout] Toolbar root={rootBounds} splitBtn={splitBounds}");
	}

	static Border MkBtn(){
		var ans = new Border{
			Background = Brushes.Black,
			BorderBrush = UiCfg.Inst.GapLineBrush,
			BorderThickness = new(0.5),
			CornerRadius = new(0),
			Padding = new(0),
			HorizontalAlignment = HAlign.Stretch,
			VerticalAlignment = VAlign.Stretch,
			ClipToBounds = true,
		};
		ans.PointerPressed += (_, _) => {
			AppLog.Debug("[SplitTouch] Toolbar cell PointerPressed");
		};
		return ans;
	}

	public void Dispose()
	{
		LayoutUpdated -= OnLayoutUpdated;
		if(_uiStatePropertyChangedHandler is not null){
			_uiState.PropertyChanged -= _uiStatePropertyChangedHandler;
		}
		(Ctx as IDisposable)?.Dispose();
	}
}
