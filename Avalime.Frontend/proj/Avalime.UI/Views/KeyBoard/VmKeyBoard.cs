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

	/// 是否顯示數字鍵盤佈局
	public bool IsNumLayout{
		get => field;
		set => SetProperty(ref field, value);
	}

	/// 上滑 $ 鍵後切換；為 true 時後續普通按鍵都帶 Shift 修飾。
	public bool IsShiftLocked{
		get => field;
		set => SetProperty(ref field, value);
	}
}
