using Avalime.Core.Keys;
using Avalime.Core.Infra.Log;
using Avalime.Rime;
using Avalime.ViewModels;
using Avalonia.Threading;
using Rime.Api;
using Tsinswreng.CsInterop;

namespace Avalime.UI.Views.input;
using Ctx = VmInput;

public class VmInput : ViewModelBase
	, IDisposable
{
	public str Text{
		get => field;
		set => SetProperty(ref field, value);
	} = "";

	public ImeState ImeState{get;set;}
	public RimeConnectionState RimeConnection{get;set;}

	readonly EventHandler<IEnumerable<IKeyEvent>> _afterInputHandler;

	unsafe public VmInput(ImeState ImeState, RimeConnectionState RimeConnection){
		this.ImeState = ImeState;
		this.RimeConnection = RimeConnection;
		_afterInputHandler = (sender, args)=>{
			var sw = System.Diagnostics.Stopwatch.StartNew();
			AppLog.Debug($"[Perf] VmInput.AfterInput start: {sw.ElapsedMilliseconds}ms");
			var rime = RimeConnection.Setup;
			if(rime is null){
				Dispatcher.UIThread.Post(() => Text = "");
				return;
			}
			var rimeApi = rime.apiFn;
			var ctx = new RimeContext();
			ctx.data_size = RimeUtil.DataSize<RimeContext>();
			if(rimeApi.get_context(rime.rimeSessionId, &ctx) != RimeUtil.True){
				return;
			}
			str preedit = ToolCStr.ToCsStr(ctx.composition.preedit);
			rimeApi.free_context(&ctx);
			Dispatcher.UIThread.Post(() => Text = preedit);
			AppLog.Debug($"[Perf] VmInput.AfterInput done: {sw.ElapsedMilliseconds}ms, preedit: {preedit}");
		};
		ImeState.AfterInput += _afterInputHandler;
	}

	public void Dispose()
	{
		ImeState.AfterInput -= _afterInputHandler;
	}
}
