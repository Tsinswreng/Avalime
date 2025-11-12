using Avalime.Core.IF;

namespace Avalime.Core.keys;


public interface I_OnKeyEvents{
	public Task<I_Result<object?>> OnKeyEventsAsy(
		IEnumerable<I_KeyEvent> keyEvents
	);
}