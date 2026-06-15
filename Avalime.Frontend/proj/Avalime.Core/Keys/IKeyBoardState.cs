namespace Avalime.Core.Keys;


public interface IKeyBoardState{//宜用單例
	public bool IsKeyDown(IKeyChar key);
	public IEnumerable<IKeyChar> AllDownKeys{get;set;}
}
