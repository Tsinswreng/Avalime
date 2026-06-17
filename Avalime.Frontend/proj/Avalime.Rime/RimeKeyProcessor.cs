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
		var resp = new RespOnKeyEvent();
		foreach (var keyEvent in KeyEvents) {
			var tuple = RimeKeyCharConverter.Inst.Convert(keyEvent);
			Rime.process_key(
				RimeSetup.rimeSessionId
				,tuple.Item1
				,tuple.Item2
			);

			// 檢查 commit
			var commit = new RimeCommit();
			commit.data_size = RimeUtil.DataSize<RimeCommit>();
			if(Rime.get_commit(RimeSetup.rimeSessionId, &commit) != RimeUtil.False){
				var text = ToolCStr.ToCsStr(commit.text);
				if(!string.IsNullOrEmpty(text)){
					resp.Commits.Add(text);
				}
				Rime.free_commit(&commit);
			}
		}
		return resp;
	}
}
