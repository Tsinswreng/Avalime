//TopBar: 候選欄 + 連接按鈕(右)
using Avalime.UI.Views.candidatesBar;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalime.UI.Infra;

namespace Avalime.UI.Views.topBar;

public class ViewTopBar : AppViewBase<VmTopBar>
{
	public ViewTopBar(){
		Ctx = VmTopBar.Mk();
		Render();
	}

	void Render(){
		var root = new Grid{
			ColumnDefinitions = new("*,Auto"),
			RowDefinitions = new("Auto,Auto")
		};

		var candidates = new ViewCandidatesBar();

		var btn = new Button{
			Content = "連接 Rime",
			HorizontalAlignment = HorizontalAlignment.Right
		};
		btn.Click += (_, _) => Ctx?.ConnectRime();

		var status = new TextBlock{
			VerticalAlignment = VerticalAlignment.Center,
			HorizontalAlignment = HorizontalAlignment.Right,
			Margin = new(0, 2, 4, 0)
		};
		Ctx!.Bind(status, x => x.Text, x => x.StatusText);

		root.Children.Add(candidates);
		root.Children.Add(btn);
		root.Children.Add(status);
		Grid.SetColumn(candidates, 0);
		Grid.SetRowSpan(candidates, 2);
		Grid.SetColumn(btn, 1);
		Grid.SetRow(status, 1);
		Grid.SetColumn(status, 1);

		this.SetContent(root);
	}
}
