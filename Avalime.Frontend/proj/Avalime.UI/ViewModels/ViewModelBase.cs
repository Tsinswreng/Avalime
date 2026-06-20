using CommunityToolkit.Mvvm.ComponentModel;
using Avalime.Core.Infra.Log;
using Tsinswreng.CsCore;

namespace Avalime.ViewModels;

public class ViewModelBase : ObservableObject
{
	//統一錯誤處理入口
	public nil HandleErr(obj? Ex){
		if(Ex is Exception ex){
			AppLogX.Error(ex);
		}else{
			AppLogX.Error(Ex?.ToString()??"");
		}
		return NIL;
	}
}
