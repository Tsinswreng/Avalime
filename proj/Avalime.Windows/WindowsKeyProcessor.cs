using Avalime.Core;
using Avalime.Core.IF;
using Avalime.Core.Keys;

namespace Avalime.Windows;


public class WindowsKeyProcessor
	:I_OsKeyProcessor
	,IImeKeyProcessor//TODO test
{

	protected static WindowsKeyProcessor? _inst = null;
	public static WindowsKeyProcessor inst => _inst??= new WindowsKeyProcessor();

	public WindowsKeyProcessor() {
	}


	public event ErrHandler? OnErr;

	public async Task<I_Result<object?>>  OnKeyEventsAsy(IEnumerable<IKeyEvent> keyEvents) {
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