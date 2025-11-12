namespace Avalime.Core.keys;


public interface I_KeyBoardState{//宜用單例
	public bool isKeyDown(I_KeyChar key);
	public IEnumerable<I_KeyChar> allDownKeys{get;set;}
}