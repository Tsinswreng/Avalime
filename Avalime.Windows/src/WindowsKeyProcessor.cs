using Avalime.Core.keys;

namespace Avalime.Windows;


public class WindowsKeyProcessor
	:I_OsKeyProcessor
	,I_ImeKeyProcessor//TODO test
{

	protected static WindowsKeyProcessor? _inst = null;
	public static WindowsKeyProcessor inst => _inst??= new WindowsKeyProcessor();

	public WindowsKeyProcessor() {
	}


	public event errHandler? errEvent;

	public zero onKeyEvent(I_KeyEvent keyEvent) {
		var ans = KeyEventConverter.inst.convertKeyEvent(keyEvent);
		KeySender.keybd_event(
			ans.Item1,
			ans.Item2,
			ans.Item3,
			ans.Item4
		);
		return 0;
	}
}