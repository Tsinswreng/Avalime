namespace Avalime.Core.keys;


public interface I_KeyBoardState{
	public bool isKeyDown(I_Key key);
	public IEnumerable<I_Key> allDownKeys{get;set;}
}