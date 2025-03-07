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
		{Keys.a, 0x41}
		,{Keys.b, 0x42}
		,{Keys.c, 0x43}
		,{Keys.d, 0x44}
		,{Keys.e, 0x45}
		,{Keys.f, 0x46}
		,{Keys.g, 0x47}
		,{Keys.h, 0x48}
		,{Keys.i, 0x49}
		,{Keys.j, 0x4A}
		,{Keys.k, 0x4B}
		,{Keys.l, 0x4C}
		,{Keys.m, 0x4D}
		,{Keys.n, 0x4E}
		,{Keys.o, 0x4F}
		,{Keys.p, 0x50}
		,{Keys.q, 0x51}
		,{Keys.r, 0x52}
		,{Keys.s, 0x53}
		,{Keys.t, 0x54}
		,{Keys.u, 0x55}
		,{Keys.v, 0x56}
		,{Keys.w, 0x57}
		,{Keys.x, 0x58}
		,{Keys.y, 0x59}
		,{Keys.z, 0x5A}
	};

	// IDictionary<string ,int> test = new Dictionary<string ,int>{
	// 	{"a", 1}
	// };

}