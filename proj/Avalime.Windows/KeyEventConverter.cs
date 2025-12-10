using Avalime.Core.Keys;

namespace Avalime.Windows;

/*
TODO
慮 KeyChars中之上檔位
轉換當成 shift按+下檔按+下檔鬆+shift鬆

 */
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
		u8 keyCode = 0;
		keyCode = (u8)lower__keyCode[keyEvent.key];
		var dwFlags = stateToDwFlags(keyEvent.keyState);
		return (keyCode, 0, 0, dwFlags);
	}

	public IDictionary<IKeyChar, i64> lower__keyCode = new Dictionary<IKeyChar, i64>{
		 {KeyChars.a, 0x41}
		,{KeyChars.b, 0x42}
		,{KeyChars.c, 0x43}
		,{KeyChars.d, 0x44}
		,{KeyChars.e, 0x45}
		,{KeyChars.f, 0x46}
		,{KeyChars.g, 0x47}
		,{KeyChars.h, 0x48}
		,{KeyChars.i, 0x49}
		,{KeyChars.j, 0x4A}
		,{KeyChars.k, 0x4B}
		,{KeyChars.l, 0x4C}
		,{KeyChars.m, 0x4D}
		,{KeyChars.n, 0x4E}
		,{KeyChars.o, 0x4F}
		,{KeyChars.p, 0x50}
		,{KeyChars.q, 0x51}
		,{KeyChars.r, 0x52}
		,{KeyChars.s, 0x53}
		,{KeyChars.t, 0x54}
		,{KeyChars.u, 0x55}
		,{KeyChars.v, 0x56}
		,{KeyChars.w, 0x57}
		,{KeyChars.x, 0x58}
		,{KeyChars.y, 0x59}
		,{KeyChars.z, 0x5A}

	};

	public IDictionary<IKeyChar, i64> fn__keyCode = new Dictionary<IKeyChar, i64>{
		 {KeyChars.Backspace, 0x08}
	};

	// IDictionary<string ,int> test = new Dictionary<string ,int>{
	// 	{"a", 1}
	// };

}