namespace Avalime.Core.Keys;

public class KeyBoardState : IKeyBoardState
{
	HashSet<IKeyChar> _downKeys = [];

	public IEnumerable<IKeyChar> AllDownKeys{
		get => _downKeys;
		set => _downKeys = value is HashSet<IKeyChar> hs ? [.. hs] : [.. value];
	}

	public bool IsKeyDown(IKeyChar key){
		return _downKeys.Contains(key);
	}

	public static KeyBoardState Mk(params IKeyChar[] downKeys){
		return new KeyBoardState{
			AllDownKeys = downKeys
		};
	}
}
