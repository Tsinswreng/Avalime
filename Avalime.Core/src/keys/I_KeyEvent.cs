namespace Avalime.Core.keys;

public interface I_KeyEvent{
	public I_Key key{get;set;}
	public I_KeyState keyState{get;set;}
	public I_KeyBoardState? keyBoardState{get;set;}
}

