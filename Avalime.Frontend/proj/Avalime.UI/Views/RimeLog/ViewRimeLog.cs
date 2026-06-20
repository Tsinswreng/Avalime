using Avalime.UI.Infra;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia;
using Avalonia.Controls.Primitives;

namespace Avalime.UI.Views.RimeLog;

public class ViewRimeLog : AppViewBase<VmRimeLog>
{
	public ViewRimeLog(VmRimeLog vm)
	{
		Ctx = vm;
		Render();
	}

	void Render()
	{
		var items = new ItemsControl();
		items.SetItemTemplate<string>((line, _) => {
			return new TextBlock{
				Text = line,
				Foreground = Brushes.White,
				TextWrapping = TextWrapping.Wrap,
				Margin = new Thickness(10, 6),
				FontSize = UiCfg.Inst.BaseFontSize,
			};
		}).SetItemsPanel(() => new StackPanel{
			Orientation = Orientation.Vertical,
			Spacing = 2,
		});
		Ctx!.Bind(items, ItemsControl.ItemsSourceProperty, x => x.Lines);

		var sc = new ScrollViewer{
			Background = Brushes.Black,
			Content = items,
			VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
		};
		this.SetContent(sc);
	}
}
