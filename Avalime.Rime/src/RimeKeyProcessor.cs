using Avalime.Core;
using Avalime.Core.IF;
using Avalime.Core.keys;
using Avalime.UI;
using Rime.Api;

namespace Avalime.Rime;


unsafe public class RimeKeyProcessor
	: I_ImeKeyProcessor
{
	public event errHandler? errEvent;
	protected RimeSetup rimeSetup = RimeSetup.inst;
	public DelegateRimeApiFn rime{get;set;}
	public RimeKeyProcessor() {
		rime = rimeSetup.rime;
	}

	public async Task<I_Result<object?>> OnKeyEventsAsy(IEnumerable<I_KeyEvent> keyEvents) {
		foreach (var keyEvent in keyEvents) {
			rime.process_key(
				rimeSetup.rimeSessionId
				,keyEvent.key.name.ToLower()[0]
				,RimeModifier.zero
			);
		}
		return Result<object?>.Ok;
	}
}
