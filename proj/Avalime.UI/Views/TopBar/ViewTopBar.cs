//TopBar: 候選欄容器
using Avalime.UI.Views.candidatesBar;
using Avalonia.Controls;
using Avalime.UI.Infra;

namespace Avalime.UI.Views.topBar;

public class ViewTopBar : AppViewBase<VmTopBar>
{
	public ViewTopBar(){
		Ctx = VmTopBar.Mk();
		Render();
	}

	void Render(){
		this.SetContent(new ViewCandidatesBar());
	}
}

public class VmTopBar
{
	public static VmTopBar Mk(){return new VmTopBar();}
}
