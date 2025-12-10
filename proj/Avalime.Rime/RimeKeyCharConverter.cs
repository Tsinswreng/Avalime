using Avalime.Core.Keys;
using Rime.Api;
using KS = Avalime.Core.Keys.KeyChars;
namespace Avalime.Rime;

public class RimeKeyCharConverter{
	protected static RimeKeyCharConverter? _inst = null;
	public static RimeKeyCharConverter inst => _inst??= new RimeKeyCharConverter();


	public (i32, i32) convert(I_KeyEvent keyEvent){
		i32 mask;
		if(keyEvent.keyState.isKeyDown){
			mask = RimeModifier.zero;
		}else{
			mask = RimeModifier.kReleaseMask;
		}
		var keyCode = (i32)lower__keyCode[keyEvent.key];//TODO handle err
		return (keyCode, mask);
	}


	public IDictionary<IKeyChar, i64> lower__keyCode = new Dictionary<IKeyChar, i64>{
		{KS.A, 0x41}
		,{KS.B, 0x42}
		,{KS.C, 0x43}
		,{KS.D, 0x44}
		,{KS.E, 0x45}
		,{KS.F, 0x46}
		,{KS.G, 0x47}
		,{KS.H, 0x48}
		,{KS.I, 0x49}
		,{KS.J, 0x4A}
		,{KS.K, 0x4B}
		,{KS.L, 0x4C}
		,{KS.M, 0x4D}
		,{KS.N, 0x4E}
		,{KS.O, 0x4F}
		,{KS.P, 0x50}
		,{KS.Q, 0x51}
		,{KS.R, 0x52}
		,{KS.S, 0x53}
		,{KS.T, 0x54}
		,{KS.U, 0x55}
		,{KS.V, 0x56}
		,{KS.W, 0x57}
		,{KS.X, 0x58}
		,{KS.Y, 0x59}
		,{KS.Z, 0x5A}

		,{KS.D0, 0x30}
		,{KS.D1, 0x31}
		,{KS.D2, 0x32}
		,{KS.D3, 0x33}
		,{KS.D4, 0x34}
		,{KS.D5, 0x35}
		,{KS.D6, 0x36}
		,{KS.D7, 0x37}
		,{KS.D8, 0x38}
		,{KS.D9, 0x39}


		,{KS.a, 0x61} // a
		,{KS.b, 0x62} // b
		,{KS.c, 0x63} // c
		,{KS.d, 0x64} // d
		,{KS.e, 0x65} // e
		,{KS.f, 0x66} // f
		,{KS.g, 0x67} // g
		,{KS.h, 0x68} // h
		,{KS.i, 0x69} // i
		,{KS.j, 0x6A} // j
		,{KS.k, 0x6B} // k
		,{KS.l, 0x6C} // l
		,{KS.m, 0x6D} // m
		,{KS.n, 0x6E} // n
		,{KS.o, 0x6F} // o
		,{KS.p, 0x70} // p
		,{KS.q, 0x71} // q
		,{KS.r, 0x72} // r
		,{KS.s, 0x73} // s
		,{KS.t, 0x74} // t
		,{KS.u, 0x75} // u
		,{KS.v, 0x76} // v
		,{KS.w, 0x77} // w
		,{KS.x, 0x78} // x
		,{KS.y, 0x79} // y
		,{KS.z, 0x7A} // z



//using KS = Avalime.Core.keys.KeyChars;
		,{KS.Tilde, 0x7E}
		,{KS.Grave, 0x60}
		,{KS.Exclamation, 0x21}
		,{KS.At, 0x40}
		,{KS.HashTag, 0x23}
		,{KS.Dollar, 0x24}
		,{KS.Percent, 0x25}
		,{KS.Caret, 0x5E}
		,{KS.Ampersand, 0x26}
		,{KS.Asterisk, 0x2A}
		,{KS.Paren_L, 0x28}
		,{KS.Paren_R, 0x29}
		,{KS.Underscore, 0x5F}
		,{KS.Minus, 0x2D}
		,{KS.Plus, 0x2B}
		,{KS.Equal, 0x3D}
		,{KS.Backspace, 65288}

		,{KS.Tab, 65289}
		,{KS.Braces_L, 123}
		,{KS.SquareBracket_L, 91}
		,{KS.Braces_R, 125}
		,{KS.SquareBracket_R, 93}
		,{KS.BackSlash, 0x5C}
		,{KS.CapsLock, 65509}
		,{KS.Colon, 0x3A}
		,{KS.Semicolon, 0x3B}
		,{KS.Quote, 0x22}
		,{KS.Apostrophe, 0x27}
		,{KS.Enter, 65293}

		,{KS.Shift_L, 65505}
		,{KS.Less, 0x2C}
		,{KS.Comma, 0x2C}
		,{KS.Greater, 0x3E}
		,{KS.Period, 0x2E}
		,{KS.Question, 0x3F}
		,{KS.Slash, 0x2F}
		,{KS.Shift_R, 65506}

		,{KS.Ctrl_L, 65507}
		,{KS.Meta_L, 65511}
		,{KS.Alt_L, 65513}
		,{KS.Space, 32}
		,{KS.Alt_R, 65514}
		,{KS.Fn, 65383}
		,{KS.Ctrl_R, 65508}

		,{KS.Up, 65362}
		,{KS.Down, 65364}
		,{KS.Left, 65361}
		,{KS.Right, 65363}
	};
}



