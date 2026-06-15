namespace Avalime.Core.Keys;

public class RespOnKeyEvent{

}

public interface I_OnKeyEvents{
	public Task<RespOnKeyEvent> OnKeyEventsAsy(
		IEnumerable<IKeyEvent> keyEvents
	);
}
