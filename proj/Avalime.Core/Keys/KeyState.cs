namespace Avalime.Core.Keys;

public class KeyState
	:IKeyState
{
	public bool IsKeyDown{get;set;}
}

public static class KeyStates{
	public static IKeyState Down = new KeyState{IsKeyDown = true};
	public static IKeyState Up = new KeyState{IsKeyDown = false};
}