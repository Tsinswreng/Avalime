namespace Avalime.Core.Keys;


public interface IRespOnKeyEvent{
	public IList<string> Commits{get;set;}
	/// Rime 未處理的按鍵，應轉發給 OS
	public IList<IKeyEvent> UnhandledKeys{get;set;}
}

public class RespOnKeyEvent:IRespOnKeyEvent{
	public IList<string> Commits{get;set;} = [];
	/// Rime 未處理的按鍵，應轉發給 OS
	public IList<IKeyEvent> UnhandledKeys{get;set;} = [];
}



public interface I_OnKeyEvents{
	public Task<IRespOnKeyEvent> OnKeyEvents(
		IEnumerable<IKeyEvent> keyEvents, CT Ct
	);
}
