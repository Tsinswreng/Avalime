using Avalime.Core.Infra;
using Avalime.UI.Infra;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;

namespace Avalime.UI.Views.ViewClipboard;
using Ctx = VmClipboard;

public class ViewClipboard : AppViewBase<Ctx>
{
	public ViewClipboard(){
		Ctx = Di.DiOrMk<Ctx>();
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
		Ctx.Bind(items, ItemsControl.ItemsSourceProperty, x => x.Items);

		this.SetContent(new ScrollViewer(), o=>{
			o.Background = Brushes.Black;
			o.SetContent(items);
		});
	}
}
