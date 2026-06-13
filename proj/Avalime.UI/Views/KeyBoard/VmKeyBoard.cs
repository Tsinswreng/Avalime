using Avalime.Core.Keys;
using Avalime.UI;
using Avalime.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Avalime.ViewModels.KeyBoard;
using Ctx = VmKeyBoard;

public partial class VmKeyBoard : ViewModelBase
{
	protected VmKeyBoard(){}
	public static Ctx Mk(){return new Ctx();}

	//TODO 改用接口
	public ImeState ImeState{get;set;} = App.SvcP.GetRequiredService<ImeState>();
}
