namespace Avalime.Core.Keys;
using Avalime.Core.Infra.Log;

public class ImeState
	//:I_ImeState //TODO 先叶後抽象
{

	public IDictionary<object, object?> Cfg{get;set;}= new Dictionary<object, object?>();
	public IOsKeyProcessor OsKeyProcessor{get;set;}
	public IImeKeyProcessor ImeKeyProcessor{get;set;}

	public ImeState(IOsKeyProcessor osKeyProcessor, IImeKeyProcessor imeKeyProcessor) {
		this.OsKeyProcessor = osKeyProcessor;
		this.ImeKeyProcessor = imeKeyProcessor;
	}

	public ImeState(IOsKeyProcessor osKeyProcessor) {
		this.OsKeyProcessor = osKeyProcessor;
	}

	public void InputSafely(IEnumerable<IKeyEvent> keyEvents, Action<Exception>? onError = null){
		_ = Task.Run(async () => {
			try{
				await Input(keyEvents);
			}catch(Exception ex){
				AppLog.Error(ex, "[ImeState] InputSafely error");
				onError?.Invoke(ex);
			}
		});
	}

	public async Task<RespInput> Input(IEnumerable<IKeyEvent> keyEvents){
		var sw = System.Diagnostics.Stopwatch.StartNew();
		AppLog.Debug($"[Perf] ImeState.Input start: {sw.ElapsedMilliseconds}ms");
		BeforeInput?.Invoke(this, keyEvents);
		RespOnKeyEvent? resp = null;
		if(ImeKeyProcessor is not null){
			var swProc = System.Diagnostics.Stopwatch.StartNew();
			resp = await ImeKeyProcessor.OnKeyEventsAsy(keyEvents);
			AppLog.Debug($"[Perf] ImeState.Input OnKeyEventsAsy done: {swProc.ElapsedMilliseconds}ms");
		}
		var swAfter = System.Diagnostics.Stopwatch.StartNew();
		AfterInput?.Invoke(this, keyEvents);
		AppLog.Debug($"[Perf] ImeState.Input AfterInput done: {swAfter.ElapsedMilliseconds}ms");

		// 觸發 commit 事件
		if(resp?.Commits is not null){
			foreach(var commitText in resp.Commits){
				OnCommit?.Invoke(this, commitText);
			}
		}

		// 未處理按鍵轉發給 OS
		if(resp?.UnhandledKeys is not null && resp.UnhandledKeys.Count > 0 && OsKeyProcessor is not null){
			var swOs = System.Diagnostics.Stopwatch.StartNew();
			await OsKeyProcessor.OnKeyEventsAsy(resp.UnhandledKeys);
			AppLog.Debug($"[Perf] ImeState.Input OsKeyProcessor done: {swOs.ElapsedMilliseconds}ms, unhandled: {resp.UnhandledKeys.Count}");
		}

		AppLog.Debug($"[Perf] ImeState.Input total done: {sw.ElapsedMilliseconds}ms");
		return new();
	}

	public event EventHandler<IEnumerable<IKeyEvent>> BeforeInput;
	public event EventHandler<IEnumerable<IKeyEvent>> AfterInput;

	/// <summary>
	/// 當 Rime 引擎 commit 文字時觸發。參數為 commit 的文字內容。
	/// </summary>
	public event EventHandler<string>? OnCommit;


	public object? GetOption() {
		throw new NotImplementedException();
	}

	public object SetOption() {
		throw new NotImplementedException();
	}
}
