using Avalime.Core.Keys;
using Avalime.Core.Infra.Log;
using Microsoft.Extensions.DependencyInjection;
using Rime.Api;
using Tsinswreng.CsInterop;

namespace Avalime.Rime;


unsafe public class RimeKeyProcessor
	: IImeKeyProcessor
{
	public event ErrHandler? OnErr;
	readonly IServiceProvider _SvcProvider;
	RimeSetup? _RimeSetup;
	public RimeKeyProcessor(IServiceProvider svcProvider) {
		_SvcProvider = svcProvider;
	}

	RimeSetup GetRimeSetup() {
		_RimeSetup ??= _SvcProvider.GetRequiredService<RimeSetup>();
		return _RimeSetup;
	}

	public async Task<IRespOnKeyEvent> OnKeyEvents(IEnumerable<IKeyEvent> KeyEvents, CT Ct) {
		var sw = System.Diagnostics.Stopwatch.StartNew();
		AppLog.Debug($"[Perf] RimeKeyProcessor start: {sw.ElapsedMilliseconds}ms");
		var resp = new RespOnKeyEvent();
		var rimeSetup = GetRimeSetup();
		var rime = rimeSetup.apiFn;
		foreach (var keyEvent in KeyEvents) {
			var tuple = RimeKeyCharConverter.Inst.Convert(keyEvent);

			var swPk = System.Diagnostics.Stopwatch.StartNew();
			var handled = rime.process_key(
				rimeSetup.rimeSessionId
				,tuple.Item1
				,tuple.Item2
			);
			AppLog.Debug($"[Perf] RimeKeyProcessor process_key({keyEvent.KeyChar.Name}, {(keyEvent.KeyState.IsKeyDown?"Down":"Up")}): {swPk.ElapsedMilliseconds}ms, handled={handled}");

			//引擎未處理之按鍵 交由OS處理
			if(handled == RimeUtil.False){
				resp.UnhandledKeys.Add(keyEvent);
			}

			// 檢查 commit 這裏的commit是沒有進translator 出候選那種 直接在processor階段就上屏的
			var swCommit = System.Diagnostics.Stopwatch.StartNew();
			var commit = new RimeCommit();
			commit.data_size = RimeUtil.DataSize<RimeCommit>();
			if(rime.get_commit(rimeSetup.rimeSessionId, &commit) != RimeUtil.False){
				var text = ToolCStr.ToCsStr(commit.text);
				if(!string.IsNullOrEmpty(text)){
					resp.Commits.Add(text);
				}
				rime.free_commit(&commit);
			}
			AppLog.Debug($"[Perf] RimeKeyProcessor get_commit: {swCommit.ElapsedMilliseconds}ms");
		}
		AppLog.Debug($"[Perf] RimeKeyProcessor total: {sw.ElapsedMilliseconds}ms, unhandled: {resp.UnhandledKeys.Count}");
		return resp;
	}
}
