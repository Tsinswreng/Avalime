using static Avalime.Core.keys.KeySymbol;
using IK = Avalime.Core.keys.I_KeySymbol;
namespace Avalime.Core.keys;

using KS = KeySymbols;

public static class KeySymbols{
	#region numbers
	public static IK Tilde{get;} = k("~");
	public static IK Grave{get;} = k("`");

	public static IK Exclamation{get;} = k("!");
	public static IK D1{get;} = k("1");

	public static IK At{get;} = k("@");
	public static IK D2{get;} = k("2");

	public static IK HashTag{get;} = k("#");
	public static IK D3{get;} = k("3");

	public static IK Dollar{get;} = k("$");
	public static IK D4{get;} = k("4");

	public static IK Percent{get;} = k("%");
	public static IK D5{get;} = k("5");

	public static IK Caret{get;} = k("^");
	public static IK D6{get;} = k("6");

	public static IK Ampersand{get;} = k("&");
	public static IK D7{get;} = k("7");

	public static IK Asterisk{get;} = k("*");
	public static IK D8{get;} = k("8");

	public static IK Paren_L{get;} = k("(");
	public static IK D9{get;} = k("9");

	public static IK Paren_R{get;} = k(")");
	public static IK D0{get;} = k("0");

	public static IK Underscore{get;} = k("_");
	public static IK Minus{get;} = k("-");

	public static IK Plus{get;} = k("+");
	public static IK Equal{get;} = k("=");
	public static IK Backspace{get;} = k(nameof(KS.Backspace));
	#endregion numbers

	#region qwer
	public static IK Tab{get;} = k("\t");
	public static IK Q{get;} = k(nameof(KS.Q));
	public static IK q{get;} = k(nameof(KS.q));
	public static IK W{get;} = k(nameof(KS.W));
	public static IK w{get;} = k(nameof(KS.w));
	public static IK E{get;} = k(nameof(KS.E));
	public static IK e{get;} = k(nameof(KS.e));
	public static IK R{get;} = k(nameof(KS.R));
	public static IK r{get;} = k(nameof(KS.r));
	public static IK T{get;} = k(nameof(KS.T));
	public static IK t{get;} = k(nameof(KS.t));
	public static IK Y{get;} = k(nameof(KS.Y));
	public static IK y{get;} = k(nameof(KS.y));
	public static IK U{get;} = k(nameof(KS.U));
	public static IK u{get;} = k(nameof(KS.u));
	public static IK I{get;} = k(nameof(KS.I));
	public static IK i{get;} = k(nameof(KS.i));
	public static IK O{get;} = k(nameof(KS.O));
	public static IK o{get;} = k(nameof(KS.o));
	public static IK P{get;} = k(nameof(KS.P));
	public static IK p{get;} = k(nameof(KS.p));
	public static IK Braces_L{get;} = k("{");
	public static IK SquareBracket_L{get;} = k("[");
	public static IK Braces_R{get;} = k("}");
	public static IK SquareBracket_R{get;} = k("]");
	public static IK BackSlash{get;} = k("\\");
	#endregion qwer

	#region asdf
	public static IK CapsLock{get;} = k(nameof(KS.CapsLock));
	public static IK A{get;} = k(nameof(KS.A));
	public static IK a{get;} = k(nameof(KS.a));
	public static IK S{get;} = k(nameof(KS.S));
	public static IK s{get;} = k(nameof(KS.s));
	public static IK D{get;} = k(nameof(KS.D));
	public static IK d{get;} = k(nameof(KS.d));
	public static IK F{get;} = k(nameof(KS.F));
	public static IK f{get;} = k(nameof(KS.f));
	public static IK G{get;} = k(nameof(KS.G));
	public static IK g{get;} = k(nameof(KS.g));
	public static IK H{get;} = k(nameof(KS.H));
	public static IK h{get;} = k(nameof(KS.h));
	public static IK J{get;} = k(nameof(KS.J));
	public static IK j{get;} = k(nameof(KS.j));
	public static IK K{get;} = k(nameof(KS.K));
	public static IK k{get;} = k(nameof(KS.k));
	public static IK L{get;} = k(nameof(KS.L));
	public static IK l{get;} = k(nameof(KS.l));

	public static IK Colon{get;} = k(":");
	public static IK Semicolon{get;} = k(";");

	public static IK Apostrophe{get;} = k("'");
	public static IK Quote{get;} = k("\"");

	public static IK Enter{get;} = k(nameof(KS.Enter));

	#endregion asdf
	#region zxcv
	public static IK Shift_L{get;} = k(nameof(KS.Shift_L));

	public static IK Z{get;} = k(nameof(KS.Z));
	public static IK z{get;} = k(nameof(KS.z));

	public static IK X{get;} = k(nameof(KS.X));
	public static IK x{get;} = k(nameof(KS.x));

	public static IK C{get;} = k(nameof(KS.C));
	public static IK c{get;} = k(nameof(KS.c));

	public static IK V{get;} = k(nameof(KS.V));
	public static IK v{get;} = k(nameof(KS.v));

	public static IK B{get;} = k(nameof(KS.B));
	public static IK b{get;} = k(nameof(KS.b));

	public static IK N{get;} = k(nameof(KS.N));
	public static IK n{get;} = k(nameof(KS.n));

	public static IK M{get;} = k(nameof(KS.M));
	public static IK m{get;} = k(nameof(KS.m));

	public static IK LessThan{get;} = k("<");
	public static IK Comma{get;} = k(",");

	public static IK GreaterThan{get;} = k(">");

	public static IK Period{get;} = k(".");

	public static IK Question{get;} = k("?");
	public static IK Slash{get;} = k("/");
	public static IK Shift_R{get;} = k(nameof(KS.Shift_R));

	#endregion zxcv

	#region Ctrl
	public static IK Ctrl_L{get;} = k(nameof(KS.Ctrl_L));
	public static IK Meta_L{get;} = k(nameof(KS.Meta_L));
	public static IK Alt_L{get;} = k(nameof(KS.Alt_L));
	public static IK Space{get;} = k(" ");
	public static IK Alt_R{get;} = k(nameof(KS.Alt_R));
	public static IK Fn{get;} = k(nameof(KS.Fn));
	public static IK Ctrl_R{get;} = k(nameof(KS.Ctrl_R));
	#endregion Ctrl

	#region Arrows
	public static IK Up{get;} = k(nameof(KS.Up));
	public static IK Down{get;} = k(nameof(KS.Down));
	public static IK Left{get;} = k(nameof(KS.Left));
	public static IK Right{get;} = k(nameof(KS.Right));
	#endregion Arrows

}