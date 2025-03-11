using System.Collections.Generic;
using System.Linq;
using Avalime.Core.keys;
using Avalime.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Rime.Api;
using static Shr.Interop.CStrUtil;

namespace Avalime.UI.views.input;
public class InputVm
	:ViewModelBase
{
	protected str _text = "";
	public str text{
		get{return _text;}
		set{SetProperty(ref _text, value);}
	}
	public ImeState imeState{get;set;} = App.ServiceProvider.GetRequiredService<ImeState>();

	unsafe public InputVm(){
		imeState.onInput += (sender, args) => {
			var rime = RimeSetup.inst;
			var rimeApi = rime.apiFn;
			var ctx = new RimeContext();
			rimeApi.get_context(rime.rimeSessionId, &ctx);
			str preedit = cStrToCsStr(ctx.composition.preedit);
			text = preedit;
			System.Console.WriteLine(text);//t
			// var sb= new List<str>();
			// foreach(var keyEvent in args){
			// 	if(keyEvent.keyState.isKeyDown){
			// 		sb.Add(keyEvent.key.name);
			// 	}
			// }
			// var txt = str.Join("", sb);
			// text += txt;
		};
	}

}