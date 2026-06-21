using Avalime.Core.Infra.Log;
using Avalime.Core.Keys;
using Avalime.ViewModels;
using Avalonia.Threading;

namespace Avalime.UI.Views.Input;
using Ctx = VmInput;

public class VmInput : ViewModelBase
	, IDisposable
{
	public str Text{
		get => field;
		set => SetProperty(ref field, value);
	} = "";

	public ISvcIme ImeState{get;}

	readonly EventHandler<IEnumerable<IKeyEvent>> _afterInputHandler;

	public VmInput(ISvcIme ImeState){
		this.ImeState = ImeState;
		_afterInputHandler = (sender, args)=>{
			var sw = System.Diagnostics.Stopwatch.StartNew();
			AppLog.Debug($"[Perf] VmInput.AfterInput start: {sw.ElapsedMilliseconds}ms");
			str preedit = ImeState.Preedit;
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
