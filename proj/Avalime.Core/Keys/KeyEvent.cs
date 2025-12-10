namespace Avalime.Core.Keys;

public class KeyEvent
	:IKeyEvent
{
	public IKeyChar Key{get;set;}
	public IKeyState KeyState{get;set;}
	public IKeyBoardState? KeyBoardState{get;set;}
}

