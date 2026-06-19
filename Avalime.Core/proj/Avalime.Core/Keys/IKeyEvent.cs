namespace Avalime.Core.Keys;

public interface IKeyEvent{
	public IKeyChar KeyChar{get;set;}
	public IKeyState KeyState{get;set;}
	public IKeyBoardState? KeyBoardState{get;set;}
}

