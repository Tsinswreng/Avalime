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
	`VmCandidatesBar` 和 `VmInput` 都依賴 `ImeState.AfterInput`。
	`VmCandidate` 支援點擊（`Click`），點擊後發送對應數字鍵選中該候選詞上屏。
	候選詞列表最多顯示 16 個。
]

""")]
file class _{
}
