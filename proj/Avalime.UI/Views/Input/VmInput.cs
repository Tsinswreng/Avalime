

using System.Collections.Generic;
using System.Linq;
using Avalime.Core.Keys;
using Avalime.Rime;
using Avalime.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Rime.Api;
using Tsinswreng.CsInterop;

namespace Avalime.UI.Views.input;



public class VmInput
	:ViewModelBase
{
	public str Text{
		get{return field;}
		set{SetProperty(ref field, value);}
	}= "";
	public ImeState imeState{get;set;} = App.SvcP.GetRequiredService<ImeState>();

	unsafe public VmInput(){
		imeState.AfterInput += (sender, args) => {
			//G.debug("InputVm");//t
			var rime = RimeSetup.Inst;
			var rimeApi = rime.apiFn;
			var ctx = new RimeContext();
			ctx.data_size = RimeUtil.DataSize<RimeContext>();
			if(rimeApi.get_context(rime.rimeSessionId, &ctx) != RimeUtil.True){
				//G.debug("get_context failed");//TODO
				return;
			}
			str preedit = ToolCStr.ToCsStr(ctx.composition.preedit);
			rimeApi.free_context(&ctx);
			Text = preedit;

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
