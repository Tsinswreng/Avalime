using Avalime.UI.Infra;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;

namespace Avalime.UI.Views.clipboard;

public class ViewClipboard : AppViewBase<VmClipboard>
{
	public ViewClipboard(VmClipboard vm){
		Ctx = vm;
		Render();
	}

	void Render(){
		var items = new ItemsControl();
		items.SetItemTemplate<VmClipboardItem>((vm, _) => {
			var btn = new Button{
				Background = Brushes.Black,
				Foreground = Brushes.White,
				CornerRadius = new(0),
				BorderBrush = SolidColorBrush.Parse("#253238"),
				BorderThickness = new(0.5),
				HorizontalAlignment = HorizontalAlignment.Stretch,
				HorizontalContentAlignment = HorizontalAlignment.Left,
				Padding = new(12, 10),
			};
			btn.Bind(Button.ContentProperty, new Avalonia.Data.Binding(nameof(VmClipboardItem.Text)));
			btn.Click += (_, _) => vm.Click?.Invoke();
			return btn;
		}).SetItemsPanel(() => new StackPanel{
			Orientation = Orientation.Vertical,
			Spacing = 2,
		});
		Ctx!.Bind(items, ItemsControl.ItemsSourceProperty, x => x.Items);

		var sc = new ScrollViewer{
			Background = Brushes.Black,
			Content = items
		};
		this.SetContent(sc);
	}
}
