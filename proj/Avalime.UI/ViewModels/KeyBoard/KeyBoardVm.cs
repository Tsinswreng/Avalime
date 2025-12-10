using System.Collections;
using System.Collections.Generic;
using Avalime.Core.IF;
using Avalime.Core.Keys;
using Avalime.UI;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
namespace Avalime.ViewModels.KeyBoard;

public partial class VmKeyBoard
	:ViewModelBase
{

	//TODO 改用接口
	public ImeState imeState{get;set;} = App.ServiceProvider.GetRequiredService<ImeState>();



	// public I_Result<object?> input(IEnumerable<I_KeyChar> keyChars) {
	// 	var ans = imeState.input(keyChars);
	// 	return ans;
	// }


}