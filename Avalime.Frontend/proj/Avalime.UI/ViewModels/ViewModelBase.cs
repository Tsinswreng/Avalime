using CommunityToolkit.Mvvm.ComponentModel;
using Tsinswreng.CsCore;

namespace Avalime.ViewModels;

public class ViewModelBase : ObservableObject
{
	//統一錯誤處理入口
	public nil HandleErr(obj? Ex){
		//TODO 接入彈窗或日誌系統
		System.Diagnostics.Debug.WriteLine(Ex?.ToString()??"");
		return NIL;
	}
}
