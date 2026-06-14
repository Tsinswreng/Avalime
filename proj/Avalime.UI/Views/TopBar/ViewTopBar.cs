//TopBar: 候選欄容器
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
			ColumnDefinitions = new("Auto,*"),
			RowDefinitions = new("Auto,Auto")
		};

		var btn = new Button{
			Content = "連接 Rime",
			HorizontalAlignment = HorizontalAlignment.Left
		};
		btn.Click += (_, _) => Ctx?.ConnectRime();

		var status = new TextBlock{
			VerticalAlignment = VerticalAlignment.Center,
			Margin = new(12, 0, 0, 0)
		};
		Ctx!.Bind(status, x => x.Text, x => x.StatusText);

		var candidates = new ViewCandidatesBar();

		root.Children.Add(btn);
		root.Children.Add(status);
		root.Children.Add(candidates);
		Grid.SetColumn(status, 1);
		Grid.SetRow(candidates, 1);
		Grid.SetColumnSpan(candidates, 2);

		this.SetContent(root);
	}
}
