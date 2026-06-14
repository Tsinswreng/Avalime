using Avalime.Core.Keys;
using Avalime.Rime;
using Avalime.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Rime.Api;
using Tsinswreng.CsInterop;

namespace Avalime.UI.Views.input;
using Ctx = VmInput;

public class VmInput : ViewModelBase
{
	public static Ctx Mk(){return new Ctx();}

	public str Text{
		get => field;
		set => SetProperty(ref field, value);
	} = "";

	public ImeState ImeState{get;set;} = App.SvcP.GetRequiredService<ImeState>();

	unsafe public VmInput(){
		ImeState.AfterInput += (sender, args)=>{
			var rime = RimeSetup.Inst;
			var rimeApi = rime.apiFn;
			var ctx = new RimeContext();
			ctx.data_size = RimeUtil.DataSize<RimeContext>();
			if(rimeApi.get_context(rime.rimeSessionId, &ctx) != RimeUtil.True){
				return;
			}
			str preedit = ToolCStr.ToCsStr(ctx.composition.preedit);
			rimeApi.free_context(&ctx);
			Text = preedit;
		};
	}
}
