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
	所有 UI 發起的按鍵現在都走 `ImeState.InputSafely(...)`，避免原生處理阻塞 UI。
	由於 `AfterInput` 可能在後台線程觸發，`VmCandidatesBar` / `VmInput` 更新綁定屬性時都需要切回 `Dispatcher.UIThread`。
	Y 鍵左滑切換 `ascii_mode`，通過 `RimeConnectionState.ToggleAsciiMode()` 在**後台線程**調用 `set_option`（防止約 350ms 的原生調用阻塞 UI 線程導致 ANR）。
	`ToggleAsciiMode` 內部使用 `Interlocked` 防止連點並發，直接計算新狀態並通過 `Dispatcher.UIThread.Post` 更新 `IsAsciiMode`。
	ASCII 模式下所有按鍵標籤顯示為小寫拉丁字母（如 Q→q、Σ→s），`KeyVm` 監聽 `IsAsciiMode` 自動切換。
]

#H[快捷鍵][
	A / Z / X / C / V / Y 鍵支援 Ctrl 組合鍵（長按或右滑）：
	- A → Ctrl+A（全選） → 底部 hint: ☑
	- Z → Ctrl+Z（撤銷） → 底部 hint: ↶
	- X → Ctrl+X（剪切） → 底部 hint: ✁
	- C → Ctrl+C（複製） → 底部 hint: ❐
	- V → Ctrl+V（粘貼） → 底部 hint: ▣
	- Y → Ctrl+Y（重做） → 底部 hint: ↷

	實現方式：`MkSendCtrlKey` 發送 Ctrl_L Down → Key Down → Key Up → Ctrl_L Up 四個按鍵事件。
	這四個事件會攜帶 `KeyBoardState`，把「當前 Ctrl 正按下」這個上下文沿輸入鏈傳下去。
	`KeyCfg.LongClickAction` 和 `KeyCfg.SwipeRightAction` 用於綁定這類自訂按鍵序列。
	Y 鍵保留頂部 `⇆` hint 提示左滑切換 ASCII 模式。
]

#H[Hint 佈局][
	每個按鍵有兩個 hint 位置：
	- `Hint`：右上角。用於數字鍵（提示長按/上滑的符號）和特殊功能鍵（如 ⇆、⇪ 等）
	- `HintBottom`：左下角。用於 Ctrl 組合鍵功能提示
	數字鍵 hint 顯示對應的符號字元（1→!, 2→@, 3→#, ...）
	`[` / `]` 的 hint 分別顯示 `{` / `}`；左右方向鍵的 hint 分別顯示 `⇤` / `⇥`
]

#H[$ 與 Shift 鎖定][
	第五排原 `Shift` 鍵改為 `$` 鍵：
	- 單擊：發送 `$`
	- 上滑：切換 `Shift` 鎖定

	`Shift` 鎖定採用持續鎖定式：
	- 上滑一次後，後續普通按鍵都會帶 `Shift` 修飾
	- 再上滑一次取消鎖定

	M 鍵保留單擊 `m`；
	滑動時發送固定字串後再補一次 `Space` 上屏：
	- 下滑：`$m,` + `Space`
	- 左滑：`$m,j` + `Space`
	- 右滑：`$m,i` + `Space`
]

""")]
file class _{
}
