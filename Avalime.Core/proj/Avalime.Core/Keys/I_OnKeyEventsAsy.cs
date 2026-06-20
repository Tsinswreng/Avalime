namespace Avalime.Core.Keys;

public class RespOnKeyEvent{
	public List<string> Commits{get;set;} = [];
	/// <summary>Rime 未處理的按鍵，應轉發給 OS</summary>
	public List<IKeyEvent> UnhandledKeys{get;set;} = [];
}



public interface I_OnKeyEvents{
	public Task<RespOnKeyEvent> OnKeyEventsAsy(
		IEnumerable<IKeyEvent> keyEvents
	);
}
