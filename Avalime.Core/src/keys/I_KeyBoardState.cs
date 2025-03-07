namespace Avalime.Core.keys;


public interface I_KeyBoardState{
	public bool isKeyDown(I_KeySymbol key);
	public IEnumerable<I_KeySymbol> allDownKeys{get;set;}
}