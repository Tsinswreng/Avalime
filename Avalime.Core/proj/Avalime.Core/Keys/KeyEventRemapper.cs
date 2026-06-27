namespace Avalime.Core.Keys;

/// <summary>
/// OS 按鍵映射器。
/// 它只在開關打開時套用規則，否則原樣透傳，避免把 UI 狀態耦合進具體宿主發送器。
/// </summary>
public class KeyEventRemapper
	: IKeyEventRemapper
{
	readonly Func<bool> _IsEnabled;
	readonly IReadOnlyList<IKeyEventRemapRule> _Rules;

	public KeyEventRemapper(Func<bool> IsEnabled, IEnumerable<IKeyEventRemapRule> Rules)
	{
		_IsEnabled = IsEnabled;
		_Rules = [.. Rules];
	}

	/// <summary>
	/// 逐個事件套用第一條命中的規則。
	/// 這樣每個事件在同一輪映射裡只會被替換一次，規則優先級也明確由列表順序決定。
	/// </summary>
	public IEnumerable<IKeyEvent> Remap(IEnumerable<IKeyEvent> KeyEvents)
	{
		if(!_IsEnabled()){
			return KeyEvents;
		}

		return RemapCore(KeyEvents);
	}

	IEnumerable<IKeyEvent> RemapCore(IEnumerable<IKeyEvent> KeyEvents)
	{
		foreach(var keyEvent in KeyEvents){
			var remapped = false;
			foreach(var rule in _Rules){
				if(!rule.TryRemap(keyEvent, out var remappedEvents)){
					continue;
				}

				foreach(var remappedEvent in remappedEvents){
					yield return remappedEvent;
				}
				remapped = true;
				break;
			}

			if(!remapped){
				yield return keyEvent;
			}
		}
	}
}
