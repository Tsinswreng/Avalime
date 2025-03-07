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

	public IDictionary<I_KeySymbol, i64> name__keyCode = new Dictionary<I_KeySymbol, i64>{
		{KeySymbols.a, 0x41}
		,{KeySymbols.b, 0x42}
		,{KeySymbols.c, 0x43}
		,{KeySymbols.d, 0x44}
		,{KeySymbols.e, 0x45}
		,{KeySymbols.f, 0x46}
		,{KeySymbols.g, 0x47}
		,{KeySymbols.h, 0x48}
		,{KeySymbols.i, 0x49}
		,{KeySymbols.j, 0x4A}
		,{KeySymbols.k, 0x4B}
		,{KeySymbols.l, 0x4C}
		,{KeySymbols.m, 0x4D}
		,{KeySymbols.n, 0x4E}
		,{KeySymbols.o, 0x4F}
		,{KeySymbols.p, 0x50}
		,{KeySymbols.q, 0x51}
		,{KeySymbols.r, 0x52}
		,{KeySymbols.s, 0x53}
		,{KeySymbols.t, 0x54}
		,{KeySymbols.u, 0x55}
		,{KeySymbols.v, 0x56}
		,{KeySymbols.w, 0x57}
		,{KeySymbols.x, 0x58}
		,{KeySymbols.y, 0x59}
		,{KeySymbols.z, 0x5A}
	};

	// IDictionary<string ,int> test = new Dictionary<string ,int>{
	// 	{"a", 1}
	// };

}