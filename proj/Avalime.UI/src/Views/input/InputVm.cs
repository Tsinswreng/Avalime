using System.Collections.Generic;
using System.Linq;
using Avalime.Core.keys;
using Avalime.Rime;
using Avalime.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Rime.Api;
using Tsinswreng.CsInterop;


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
		imeState.afterInput += (sender, args) => {
			//G.debug("InputVm");//t
			var rime = RimeSetup.inst;
			var rimeApi = rime.apiFn;
			var ctx = new RimeContext();
			ctx.data_size = RimeUtil.dataSize<RimeContext>();
			if(rimeApi.get_context(rime.rimeSessionId, &ctx) != RimeUtil.True){
				//G.debug("get_context failed");//TODO
				return;
			}
			str preedit = ToolCStr.ToCsStr(ctx.composition.preedit);
			rimeApi.free_context(&ctx);
			text = preedit;

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
