namespace Avalime.Core.Keys;


public interface I_KeyBoardState{//宜用單例
	public bool isKeyDown(IKeyChar key);
	public IEnumerable<IKeyChar> allDownKeys{get;set;}
}