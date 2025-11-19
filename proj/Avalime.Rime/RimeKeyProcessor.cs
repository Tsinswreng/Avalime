using Avalime.Core;
using Avalime.Core.IF;
using Avalime.Core.keys;
using Rime.Api;

namespace Avalime.Rime;


unsafe public class RimeKeyProcessor
	: I_ImeKeyProcessor
{
	public event errHandler? errEvent;
	protected RimeSetup rimeSetup = RimeSetup.inst;
	public RimeApi rime{get;set;}
	public RimeKeyProcessor() {
		rime = rimeSetup.apiFn;
	}

	public async Task<I_Result<object?>> OnKeyEventsAsy(IEnumerable<I_KeyEvent> keyEvents) {
		foreach (var keyEvent in keyEvents) {
			var tuple = RimeKeyCharConverter.inst.convert(keyEvent);
			rime.process_key(
				rimeSetup.rimeSessionId
				,tuple.Item1
				,tuple.Item2
			);
		}
		return Result<object?>.Ok;
	}
}
