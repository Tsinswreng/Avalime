using System.Diagnostics;
using Android.Views.InputMethods;
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
			Debug.WriteLine($"[IME] OsKeyProcessor forwarding: {name}");

			var androidKey = MapToAndroidKey(name);
			if(androidKey is not null){
				ic.SendKeyEvent(new global::Android.Views.KeyEvent(global::Android.Views.KeyEventActions.Down, androidKey.Value));
				ic.SendKeyEvent(new global::Android.Views.KeyEvent(global::Android.Views.KeyEventActions.Up, androidKey.Value));
			}else{
				var text = KeyNameToText(name);
				if(text is not null){
					ic.CommitText(text, 1);
				}
			}
		}
		return Task.FromResult(new RespOnKeyEvent());
	}

	// 將 IKeyChar.Name 映射到 Android Keycode
	static global::Android.Views.Keycode? MapToAndroidKey(string name) => name switch {
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
