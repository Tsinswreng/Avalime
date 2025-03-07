namespace Avalime.Core.keys;


public interface I_KeyBoardState{
	public bool isKeyDown(I_KeyChar key);
	public IEnumerable<I_KeyChar> allDownKeys{get;set;}
}