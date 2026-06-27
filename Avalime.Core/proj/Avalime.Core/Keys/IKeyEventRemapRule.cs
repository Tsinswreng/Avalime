namespace Avalime.Core.Keys;

/// <summary>
/// 單條按鍵映射規則。
/// 規則自身只關心“某個事件是否需要替換”，不負責總體開關與後續轉發。
/// </summary>
public interface IKeyEventRemapRule
{
	/// <summary>
	/// 嘗試將一個按鍵事件映射成新的事件序列。
	/// </summary>
	public bool TryRemap(IKeyEvent SourceEvent, out IEnumerable<IKeyEvent> RemappedEvents);
}
