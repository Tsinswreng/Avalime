using Avalime.Core.Keys;
using Rime.Api;
using Tsinswreng.CsInterop;

namespace Avalime.Rime;


unsafe public class RimeKeyProcessor
	: IImeKeyProcessor
{
	public event ErrHandler? OnErr;
	protected RimeSetup RimeSetup;
	public RimeApi Rime{get;set;}
	public RimeKeyProcessor()
		: this(RimeSetup.Inst){}

	public RimeKeyProcessor(RimeSetup setup) {
		RimeSetup = setup;
		Rime = RimeSetup.apiFn;
	}

	public async Task<RespOnKeyEvent> OnKeyEventsAsy(IEnumerable<IKeyEvent> KeyEvents) {
		var sw = System.Diagnostics.Stopwatch.StartNew();
		System.Diagnostics.Debug.WriteLine($"[Perf] RimeKeyProcessor start: {sw.ElapsedMilliseconds}ms");
		var resp = new RespOnKeyEvent();
		foreach (var keyEvent in KeyEvents) {
			var tuple = RimeKeyCharConverter.Inst.Convert(keyEvent);

			var swPk = System.Diagnostics.Stopwatch.StartNew();
			Rime.process_key(
				RimeSetup.rimeSessionId
				,tuple.Item1
				,tuple.Item2
			);
			System.Diagnostics.Debug.WriteLine($"[Perf] RimeKeyProcessor process_key({keyEvent.KeyChar.Name}, {(keyEvent.KeyState.IsKeyDown?"Down":"Up")}): {swPk.ElapsedMilliseconds}ms");

			// 檢查 commit
			var swCommit = System.Diagnostics.Stopwatch.StartNew();
			var commit = new RimeCommit();
			commit.data_size = RimeUtil.DataSize<RimeCommit>();
			if(Rime.get_commit(RimeSetup.rimeSessionId, &commit) != RimeUtil.False){
				var text = ToolCStr.ToCsStr(commit.text);
				if(!string.IsNullOrEmpty(text)){
					resp.Commits.Add(text);
				}
				Rime.free_commit(&commit);
			}
			System.Diagnostics.Debug.WriteLine($"[Perf] RimeKeyProcessor get_commit: {swCommit.ElapsedMilliseconds}ms");
		}
		System.Diagnostics.Debug.WriteLine($"[Perf] RimeKeyProcessor total: {sw.ElapsedMilliseconds}ms");
		return resp;
	}
}
