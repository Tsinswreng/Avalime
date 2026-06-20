namespace Avalime.Windows;

using Avalime.Core.Keys;
using Avalime.UI;

class StubImeKeyProcessor : IImeKeyProcessor
{
	public event ErrHandler? OnErr;
	public Task<RespOnKeyEvent> OnKeyEventsAsy(IEnumerable<IKeyEvent> keyEvents)
		=> Task.FromResult(new RespOnKeyEvent());
}

class StubKeyboardHost : IKeyboardHost
{
	public void HideKeyboard(){}
	public void CommitText(str text){}
}

class StubClipboardService : IClipboardService
{
	public Task<IReadOnlyList<str>> GetItemsAsy(CT ct = default)
		=> Task.FromResult<IReadOnlyList<str>>([]);
}
