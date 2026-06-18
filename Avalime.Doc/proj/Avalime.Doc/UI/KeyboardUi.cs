namespace Avalime.Doc.UI;

using Tsinswreng.CsCore;

[Doc($$"""
#H[用途][
	UI 端分成主視圖、候選欄、輸入欄、頂欄和按鍵組件。
	`MainView` 直接掛 `ViewKeyBoard`。
]

#H[鍵盤結構][
	`ViewKeyBoard` 是主鍵盤視圖。
	它同時組裝：
	- `ViewInput`
	- `ViewTopBar`
	- 主鍵盤 / 數字鍵盤切換區
]

#H[交互][
	`VmKeyBoard` 維持佈局狀態。
	`VmKey` 負責按鍵、長按與滑動動作。
	`KeyCfg.IsRepeat` 控制長按後是否持續重複：長按 400ms 後首次觸發 `LongPress`，之後每 50ms 觸發一次 `Click`。
	退格鍵（`Backspace`）設了 `IsRepeat=true`，長按可連續刪除。
	`KeyCfg.SwipeLeftAction` 支援自訂滑動動作（不限於發送按鍵），用於 Y 鍵左滑切換 ASCII 模式等場景。
	`VmCandidatesBar` 和 `VmInput` 都依賴 `ImeState.AfterInput`。
	`VmCandidate` 支援點擊（`Click`），點擊後發送對應數字鍵選中該候選詞上屏。
	候選詞列表最多顯示 16 個。
	Y 鍵左滑切換 `ascii_mode`，通過 `RimeConnectionState.ToggleAsciiMode()` 在**後台線程**調用 `set_option`（防止約 350ms 的原生調用阻塞 UI 線程導致 ANR）。
	`ToggleAsciiMode` 內部使用 `Interlocked` 防止連點並發，直接計算新狀態並通過 `Dispatcher.UIThread.Post` 更新 `IsAsciiMode`。
	ASCII 模式下所有按鍵標籤顯示為小寫拉丁字母（如 Q→q、Σ→s），`KeyVm` 監聽 `IsAsciiMode` 自動切換。
]

""")]
file class _{
}
