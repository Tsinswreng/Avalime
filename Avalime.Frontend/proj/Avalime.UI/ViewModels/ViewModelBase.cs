using CommunityToolkit.Mvvm.ComponentModel;
using Avalime.Core.Infra.Log;
using Tsinswreng.CsCore;

namespace Avalime.ViewModels;

public class ViewModelBase : ObservableObject
{
	//統一錯誤處理入口
	public nil HandleErr(obj? Ex){
		if(Ex is Exception ex){
			AppLog.Error(ex);
		}else{
			AppLog.Error(Ex?.ToString()??"");
		}
		return NIL;
	}
}
