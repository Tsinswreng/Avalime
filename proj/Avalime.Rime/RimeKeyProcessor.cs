using Avalime.Core.Keys;
using Rime.Api;

namespace Avalime.Rime;


unsafe public class RimeKeyProcessor
	: IImeKeyProcessor
{
	public event ErrHandler? OnErr;
	protected RimeSetup RimeSetup = RimeSetup.Inst;
	public RimeApi Rime{get;set;}
	public RimeKeyProcessor() {
		Rime = RimeSetup.apiFn;
	}

	public async Task<RespOnKeyEvent> OnKeyEventsAsy(IEnumerable<IKeyEvent> KeyEvents) {
		foreach (var keyEvent in KeyEvents) {
			var tuple = RimeKeyCharConverter.Inst.Convert(keyEvent);
			Rime.process_key(
				RimeSetup.rimeSessionId
				,tuple.Item1
				,tuple.Item2
			);
		}
		return new();
	}
}
