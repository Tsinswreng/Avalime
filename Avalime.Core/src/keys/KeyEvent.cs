namespace Avalime.Core.keys;

public class KeyEvent
	:I_KeyEvent
{
	public I_KeySymbol key{get;set;}
	public I_KeyState keyState{get;set;}
	public I_KeyBoardState? keyBoardState{get;set;}
}

