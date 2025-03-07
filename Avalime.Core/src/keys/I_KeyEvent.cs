namespace Avalime.Core.keys;

public interface I_KeyEvent{
	public I_KeyChar key{get;set;}
	public I_KeyState keyState{get;set;}
	public I_KeyBoardState? keyBoardState{get;set;}
}

