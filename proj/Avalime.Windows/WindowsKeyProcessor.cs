using Avalime.Core;
using Avalime.Core.IF;
using Avalime.Core.Keys;

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

	public async Task<I_Result<object?>>  OnKeyEventsAsy(IEnumerable<I_KeyEvent> keyEvents) {
		foreach (var keyEvent in keyEvents) {
			var ans = KeyEventConverter.inst.convertKeyEvent(keyEvent);
			KeySender.keybd_event(
				ans.Item1,
				ans.Item2,
				ans.Item3,
				ans.Item4
			);
		}
		return Result<object?>.Ok;
	}
}