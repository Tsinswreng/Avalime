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
	`VmCandidatesBar` 和 `VmInput` 都依賴 `ImeState.AfterInput`。
]

""")]
file class _{
}
