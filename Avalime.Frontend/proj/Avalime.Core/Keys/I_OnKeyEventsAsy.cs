namespace Avalime.Core.Keys;

public class RespOnKeyEvent{
	public List<string> Commits{get;set;} = [];
}

public interface I_OnKeyEvents{
	public Task<RespOnKeyEvent> OnKeyEventsAsy(
		IEnumerable<IKeyEvent> keyEvents
	);
}
