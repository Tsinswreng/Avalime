using Avalime.Core.IF;

namespace Avalime.Core.Keys;


public interface I_OnKeyEvents{
	public Task<I_Result<object?>> OnKeyEventsAsy(
		IEnumerable<IKeyEvent> keyEvents
	);
}