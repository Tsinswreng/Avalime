namespace Avalime.Core.Keys;

public class KeyEvent
	:I_KeyEvent
{
	public IKeyChar key{get;set;}
	public I_KeyState keyState{get;set;}
	public I_KeyBoardState? keyBoardState{get;set;}
}

