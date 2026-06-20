using Avalime.UI.Icons;
using Avalime.UI.Infra;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;

namespace Avalime.UI.Views.toolbar;

public class ViewToolBar : AppViewBase<VmToolBar>
{
	public ViewToolBar(VmToolBar vm){
		Ctx = vm;
		Render();
	}

	void Render(){
		var root = new Grid{
			ColumnDefinitions = new("*,*,*,*,*,*,*,*,*,*"),
			Background = Brushes.Black,
			Height = UiCfg.Inst.TopBarHeight,
		};

		var btnHan = MkBtn();
		btnHan.Child = new TextBlock{
			Foreground = Brushes.White,
			VerticalAlignment = VAlign.Center,
			HorizontalAlignment = HAlign.Center,
			FontSize = UiCfg.Inst.TopBarFontSize,
		};
		Ctx!.Bind((TextBlock)btnHan.Child, TextBlock.TextProperty, x => x.HanLabel);
		btnHan.PointerPressed += (_, e) => {
			e.Handled = true;
			Ctx?.ToggleSimplification();
		};
		Grid.SetColumn(btnHan, 0);

		var btnClipboard = MkBtn();
		var icon = Avalime.UI.Icons.Icons.Clipboard().ToIcon();
		icon.Width = icon.Height = UiCfg.Inst.TopBarFontSize;
		btnClipboard.Child = icon;
		btnClipboard.PointerPressed += (_, e) => {
			e.Handled = true;
			Ctx?.ToggleClipboard();
		};
		Grid.SetColumn(btnClipboard, 1);

		var btnLog = MkBtn();
		var logIcon = Avalime.UI.Icons.Icons.ScrollText().ToIcon();
		logIcon.Width = logIcon.Height = UiCfg.Inst.TopBarFontSize;
		btnLog.Child = logIcon;
		btnLog.PointerPressed += (_, e) => {
			e.Handled = true;
			Ctx?.ToggleRimeLog();
		};
		Grid.SetColumn(btnLog, 2);

		root.Children.Add(btnHan);
		root.Children.Add(btnClipboard);
		root.Children.Add(btnLog);
		this.SetContent(root);
	}

	static Border MkBtn(){
		return new Border{
			Background = Brushes.Black,
			BorderBrush = SolidColorBrush.Parse("#253238"),
			BorderThickness = new(0.5),
			CornerRadius = new(0),
			Padding = new(0),
			HorizontalAlignment = HAlign.Stretch,
			VerticalAlignment = VAlign.Stretch,
		};
	}
}
