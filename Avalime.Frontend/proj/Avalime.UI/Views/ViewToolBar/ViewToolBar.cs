using Avalime.Core.Infra;
using Avalime.UI.Infra;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;

namespace Avalime.UI.Views.ViewToolBar;
using Ctx = VmToolBar;

public class ViewToolBar : AppViewBase<Ctx>
{
	public ViewToolBar(){
		Ctx = Di.DiOrMk<Ctx>();
		Render();
	}

	void Render(){
		var root = new Grid();
		root.ColumnDefinitions = new("*,*,*,*,*,*,*,*,*,*");
		root.Background = Brushes.Black;
		root.Height = UiCfg.Inst.TopBarHeight;

		var btnHan = MkBtn();
		btnHan.SetChild(new TextBlock(), o=>{
			o.Foreground = Brushes.White;
			o.VerticalAlignment = VAlign.Center;
			o.HorizontalAlignment = HAlign.Center;
			o.FontSize = UiCfg.Inst.TopBarFontSize;
		});
		Ctx.Bind((TextBlock)btnHan.Child!, TextBlock.TextProperty, x => x.HanLabel);
		btnHan.PointerPressed += (_, e) => {
			e.Handled = true;
			Ctx?.ToggleSimplification();
		};
		Grid.SetColumn(btnHan, 0);

		var btnClipboard = MkBtn();
		var icon = Avalime.UI.Icons.Icons.Clipboard();
		icon.Width = UiCfg.Inst.TopBarFontSize;
		icon.Height = UiCfg.Inst.TopBarFontSize;
		btnClipboard.SetChild(icon);
		btnClipboard.PointerPressed += (_, e) => {
			e.Handled = true;
			Ctx?.ToggleClipboard();
		};
		Grid.SetColumn(btnClipboard, 1);

		var btnLog = MkBtn();
		var logIcon = Avalime.UI.Icons.Icons.ScrollText();
		logIcon.Width = UiCfg.Inst.TopBarFontSize;
		logIcon.Height = UiCfg.Inst.TopBarFontSize;
		btnLog.SetChild(logIcon);
		btnLog.PointerPressed += (_, e) => {
			e.Handled = true;
			Ctx?.ToggleRimeLog();
		};
		Grid.SetColumn(btnLog, 2);

		root
		.A(btnHan)
		.A(btnClipboard)
		.A(btnLog)
		;
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
