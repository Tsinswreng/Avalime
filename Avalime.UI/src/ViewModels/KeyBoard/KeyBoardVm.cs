using System.Collections;
using System.Collections.Generic;
using Avalime.Core.IF;
using Avalime.Core.keys;
using Avalime.UI;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
namespace Avalime.ViewModels.KeyBoard;

public partial class KeyBoardVm
	:ViewModelBase
{

	public I_ImeState imeState{get;set;} = App.ServiceProvider.GetRequiredService<ImeState>();

	public I_Result<object?> input(IEnumerable<I_KeyChar> keyChars) {
		var ans = imeState.input(keyChars);

		return ans;
	}


}