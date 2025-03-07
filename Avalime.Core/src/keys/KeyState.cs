namespace Avalime.Core.keys;

public class KeyState
	:I_KeyState
{
	public bool isKeyDown{get;set;}
}

public static class KeyStates{
	public static I_KeyState Down = new KeyState{isKeyDown = true};
	public static I_KeyState Up = new KeyState{isKeyDown = false};
}