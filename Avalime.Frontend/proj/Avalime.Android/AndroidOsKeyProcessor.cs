using Android.Views.InputMethods;
using Avalime.Core.Infra.Log;
using Avalime.Core.Keys;

namespace Avalime.Android;

public class AndroidOsKeyProcessor : I_OsKeyProcessor
{
	readonly Func<IInputConnection?> _getInputConnection;

	public AndroidOsKeyProcessor(Func<IInputConnection?> getInputConnection){
		_getInputConnection = getInputConnection;
	}

	public event ErrHandler? OnErr;

	public Task<RespOnKeyEvent> OnKeyEventsAsy(IEnumerable<IKeyEvent> keyEvents){
		var ic = _getInputConnection();
		if(ic is null) return Task.FromResult(new RespOnKeyEvent());

		foreach(var keyEvent in keyEvents){
			if(!keyEvent.KeyState.IsKeyDown) continue;

			var name = keyEvent.KeyChar.Name;
			var metaState = ToAndroidMetaState(keyEvent.KeyBoardState);
			AppLogX.Debug($"[IME] OsKeyProcessor forwarding: {name}");

			var androidKey = MapToAndroidKey(name);
			if(androidKey is not null && (metaState != global::Android.Views.MetaKeyStates.None || ShouldSendAsKeyEvent(name))){
				ic.SendKeyEvent(new global::Android.Views.KeyEvent(0, 0, global::Android.Views.KeyEventActions.Down, androidKey.Value, 0, metaState));
				ic.SendKeyEvent(new global::Android.Views.KeyEvent(0, 0, global::Android.Views.KeyEventActions.Up, androidKey.Value, 0, metaState));
			}else{
				var text = KeyNameToText(name);
				if(text is not null){
					ic.CommitText(text, 1);
				}
			}
		}
		return Task.FromResult(new RespOnKeyEvent());
	}

	static global::Android.Views.MetaKeyStates ToAndroidMetaState(IKeyBoardState? keyBoardState){
		if(keyBoardState is null){
			return global::Android.Views.MetaKeyStates.None;
		}

		var metaState = global::Android.Views.MetaKeyStates.None;
		if(keyBoardState.IsKeyDown(KeyChars.Ctrl_L) || keyBoardState.IsKeyDown(KeyChars.Ctrl_R)){
			metaState |= global::Android.Views.MetaKeyStates.CtrlOn | global::Android.Views.MetaKeyStates.CtrlLeftOn;
		}
		if(keyBoardState.IsKeyDown(KeyChars.Shift_L) || keyBoardState.IsKeyDown(KeyChars.Shift_R)){
			metaState |= global::Android.Views.MetaKeyStates.ShiftOn;
		}
		if(keyBoardState.IsKeyDown(KeyChars.Alt_L) || keyBoardState.IsKeyDown(KeyChars.Alt_R)){
			metaState |= global::Android.Views.MetaKeyStates.AltOn;
		}
		if(keyBoardState.IsKeyDown(KeyChars.Meta_L)){
			metaState |= global::Android.Views.MetaKeyStates.MetaOn;
		}
		return metaState;
	}

	static bool ShouldSendAsKeyEvent(string name){
		return name is "Backspace" or "Enter" or "Tab" or "Left" or "Right" or "Up" or "Down"
			|| name.Length == 1 && char.IsAsciiLetterOrDigit(name[0]);
	}

	// 將 IKeyChar.Name 映射到 Android Keycode
	static global::Android.Views.Keycode? MapToAndroidKey(string name) => name switch {
		"A" or "a" => global::Android.Views.Keycode.A,
		"B" or "b" => global::Android.Views.Keycode.B,
		"C" or "c" => global::Android.Views.Keycode.C,
		"D" or "d" => global::Android.Views.Keycode.D,
		"E" or "e" => global::Android.Views.Keycode.E,
		"F" or "f" => global::Android.Views.Keycode.F,
		"G" or "g" => global::Android.Views.Keycode.G,
		"H" or "h" => global::Android.Views.Keycode.H,
		"I" or "i" => global::Android.Views.Keycode.I,
		"J" or "j" => global::Android.Views.Keycode.J,
		"K" or "k" => global::Android.Views.Keycode.K,
		"L" or "l" => global::Android.Views.Keycode.L,
		"M" or "m" => global::Android.Views.Keycode.M,
		"N" or "n" => global::Android.Views.Keycode.N,
		"O" or "o" => global::Android.Views.Keycode.O,
		"P" or "p" => global::Android.Views.Keycode.P,
		"Q" or "q" => global::Android.Views.Keycode.Q,
		"R" or "r" => global::Android.Views.Keycode.R,
		"S" or "s" => global::Android.Views.Keycode.S,
		"T" or "t" => global::Android.Views.Keycode.T,
		"U" or "u" => global::Android.Views.Keycode.U,
		"V" or "v" => global::Android.Views.Keycode.V,
		"W" or "w" => global::Android.Views.Keycode.W,
		"X" or "x" => global::Android.Views.Keycode.X,
		"Y" or "y" => global::Android.Views.Keycode.Y,
		"Z" or "z" => global::Android.Views.Keycode.Z,
		"0" => global::Android.Views.Keycode.Num0,
		"1" => global::Android.Views.Keycode.Num1,
		"2" => global::Android.Views.Keycode.Num2,
		"3" => global::Android.Views.Keycode.Num3,
		"4" => global::Android.Views.Keycode.Num4,
		"5" => global::Android.Views.Keycode.Num5,
		"6" => global::Android.Views.Keycode.Num6,
		"7" => global::Android.Views.Keycode.Num7,
		"8" => global::Android.Views.Keycode.Num8,
		"9" => global::Android.Views.Keycode.Num9,
		"Backspace" => global::Android.Views.Keycode.Del,
		"Enter" => global::Android.Views.Keycode.Enter,
		"Tab" => global::Android.Views.Keycode.Tab,
		"Left" => global::Android.Views.Keycode.DpadLeft,
		"Right" => global::Android.Views.Keycode.DpadRight,
		"Up" => global::Android.Views.Keycode.DpadUp,
		"Down" => global::Android.Views.Keycode.DpadDown,
		_ => null
	};

	// 將 IKeyChar.Name 轉換為要 commit 的文字
	static string? KeyNameToText(string name) => name switch {
		"" => null,
		" " => " ",
		"Backspace" => null,
		"Enter" => null,
		"Tab" => null,
		"Shift_L" => null,
		"Shift_R" => null,
		"Ctrl_L" => null,
		"Ctrl_R" => null,
		"Alt_L" => null,
		"Alt_R" => null,
		"Meta_L" => null,
		"Fn" => null,
		"CapsLock" => null,
		"Left" => null,
		"Right" => null,
		"Up" => null,
		"Down" => null,
		_ => name
	};
}
