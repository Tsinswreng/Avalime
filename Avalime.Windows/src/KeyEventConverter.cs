using Avalime.Core.keys;

namespace Avalime.Windows;


public class KeyEventConverter{
	protected static KeyEventConverter? _inst = null;
	public static KeyEventConverter inst => _inst??= new KeyEventConverter();

	public i32 stateToDwFlags(I_KeyState keyState){
		if(keyState.isKeyDown){
			return 0;
		}else{
			return 2;
		}
	}

	public (u8, u8, i32, i32) convertKeyEvent(
		I_KeyEvent keyEvent
	){
		var keyCode = (u8)name__keyCode[keyEvent.key];
		var dwFlags = stateToDwFlags(keyEvent.keyState);
		return (keyCode, 0, 0, dwFlags);
	}

	public IDictionary<I_Key, i64> name__keyCode = new Dictionary<I_Key, i64>{
		{Keys.A, 0x41}
		,{Keys.B, 0x42}
		,{Keys.C, 0x43}
		,{Keys.D, 0x44}
		,{Keys.E, 0x45}
		,{Keys.F, 0x46}
		,{Keys.G, 0x47}
		,{Keys.H, 0x48}
		,{Keys.I, 0x49}
		,{Keys.J, 0x4A}
		,{Keys.K, 0x4B}
		,{Keys.L, 0x4C}
		,{Keys.M, 0x4D}
		,{Keys.N, 0x4E}
		,{Keys.O, 0x4F}
		,{Keys.P, 0x50}
		,{Keys.Q, 0x51}
		,{Keys.R, 0x52}
		,{Keys.S, 0x53}
		,{Keys.T, 0x54}
		,{Keys.U, 0x55}
		,{Keys.V, 0x56}
		,{Keys.W, 0x57}
		,{Keys.X, 0x58}
		,{Keys.Y, 0x59}
		,{Keys.Z, 0x5A}
	};

	// IDictionary<string ,int> test = new Dictionary<string ,int>{
	// 	{"a", 1}
	// };

}