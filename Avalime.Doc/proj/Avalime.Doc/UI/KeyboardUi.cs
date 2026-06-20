namespace Avalime.Doc.UI;

using Tsinswreng.CsCore;

[Doc($$"""
#H[用途][
	UI 端分成 `ViewIme`、候選欄、工具欄、預編輯欄、剪貼板頁和純鍵盤。
	`MainView` 直接掛 `ViewIme`。
	候選詞完整鏈路與生命週期另見 `CandidateLifecycle`。
]

#H[鍵盤結構][
	`ViewIme` 是輸入法總視圖。
	它組裝：
	- `ViewPreedit`
	- `ViewToolBar`
	- `ViewCandidatesBar`
	- `ViewClipboard`
	- `ViewKeyBoard`

	`ViewKeyBoard` 現在只負責純鍵盤按鍵區。
	`VmIme` 是總控狀態：
	- 自動異步連接 Rime
	- 根據 `RimeStatus.is_composing` 決定顯示工具欄還是候選欄
	- 控制是否進入剪貼板頁
]

#H[DI 分層][
	當前 UI 層依賴注入規則是：
	- **只有 View 層可以直接調全局 `Di.GetRSvc<T>()`**
	- `Di` 是唯一的全局 DI 入口；不要再經由 `App` 暴露 service locator
	- `Vm` / `RimeConnectionState` / 其他非 View 類型一律使用構造函數注入
	- **所有 View 都必須提供 `public` 無參構造函數**
	- View 本身視爲無狀態；不要把 `Vm` 或其他運行時狀態通過 View 構造參數傳入
	- 除 `MainView` / `MainWindow` 這種項目模板自帶入口外，`Views/` 下每個 View/Vm 都必須放在自己的專屬同名文件夾中（如 `Views/ViewIme/ViewIme.cs`、`Views/ViewIme/VmIme.cs`）

	也就是說：
	- `ViewIme` / `ViewKeyBoard` / `ViewInput` 這些 View 內部只負責通過 `Di.DiOrMk<Ctx>()` 解析自身 `Ctx`
	- 子 View 也各自解析自己的 `Ctx`；不要在父 View 中手動 `new VmXxx(...)` 再塞給子 View
	- 禁止寫成 `new ViewXxx(vm)` 這種帶狀態構造方式
	- `VmIme` / `VmCandidatesBar` / `VmInput` / `VmClipboard` / `VmToolBar` / `VmKey` / `VmRimeLog` 內部都不應再直接碰 `Di`
	- `RimeConnectionState` 這類 service 內部若需要 `ImeState` 等依賴，也應在構造函數注入，不可反查全局容器

	這條約束是爲了把 View 與非 View 的依賴邊界固定住，避免 VM / service 變成隱式 service locator。
]

#H[交互][
	`VmKeyBoard` 維持佈局狀態。
	`VmKey` 負責按鍵、長按與滑動動作。
	`KeyCfg.IsRepeat` 控制長按後是否持續重複：長按 400ms 後首次觸發 `LongPress`，之後每 50ms 觸發一次 `Click`。
	退格鍵（`Backspace`）設了 `IsRepeat=true`，長按可連續刪除。
	`KeyCfg.SwipeLeftAction` 支援自訂滑動動作（不限於發送按鍵），用於 Y 鍵左滑切換 ASCII 模式等場景。
	`VmCandidatesBar` 和 `VmInput` 都依賴 `ImeState.AfterInput`。
	`VmIme` 也依賴 `ImeState.AfterInput`，並通過 `get_status` 讀 `is_composing`。
	`VmCandidate` 支援點擊（`Click`），點擊後發送對應數字鍵選中該候選詞上屏。
	候選詞列表最多顯示 16 個。
	所有 UI 發起的按鍵現在都走 `ImeState.InputSafely(...)`，避免原生處理阻塞 UI。
	由於 `AfterInput` 可能在後台線程觸發，`VmIme` / `VmCandidatesBar` / `VmInput` 更新綁定屬性時都需要切回 `Dispatcher.UIThread`。
	Y 鍵左滑切換 `ascii_mode`，通過 `RimeConnectionState.ToggleAsciiMode()` 在**後台線程**調用 `set_option`（防止約 350ms 的原生調用阻塞 UI 線程導致 ANR）。
	`ToggleAsciiMode` 內部使用 `Interlocked` 防止連點並發，直接計算新狀態並通過 `Dispatcher.UIThread.Post` 更新 `IsAsciiMode`。
	ASCII 模式下所有按鍵標籤顯示為小寫拉丁字母（如 Q→q、Σ→s），`VmKey` 監聽 `IsAsciiMode` 自動切換。
	工具欄第一個按鈕切換 `simplification` option：
	- `false` 時顯示 `漢`
	- `true` 時顯示 `汉`

	工具欄第二個按鈕切換剪貼板頁。
	目前先接平台當前 clipboard 內容，不含歷史。
	點擊條目後通過宿主接口 `IKeyboardHost.CommitText(...)` 直接上屏，然後退出剪貼板頁。
	工具欄還有一個日誌按鈕，點擊後在鍵盤區與 `Rime` 日誌頁之間切換；
	切換範圍只覆蓋 `ViewKeyBoard` 所在的 body 區，預編輯欄、候選欄、工具欄本身不切。
	當 `RimeConnectionState` 處於 `IsConnecting=true` 或尚未 `IsConnected` 時，也會自動切到日誌頁，方便直接觀察引擎連接輸出。
]

#H[Rime 日誌頁][
	`RimeSetup` 現在把引擎通知回調包成 C# 事件 `OnLog` / `OnOptionChanged`。
	`RimeLogBuffer` 訂閱 `RimeSetup.OnLog`，一邊把內容寫到 `AppLog.Inst`，一邊保存在內存環形列表裡給 UI 顯示。
	工具欄的日誌按鈕切換的是 `VmIme.IsRimeLogVisible`；
	其互斥關係和剪貼板頁相同：打開日誌頁時會關閉剪貼板頁，反之亦然。
]

#H[重建與釋放][
	Android IME 宿主現在在每次 hide 後，會於下次 show 前重建整棵 `AvaloniaView`。
	因此 UI 端不能再假定整個鍵盤只會初始化一次；凡是掛在全局狀態上的事件都必須可解除。

	目前釋放鏈是：
	- `AvalimeInputMethodService.OnCreateInputView()` 重建前先 `Dispose` 舊 `MainView`
	- `MainView.Dispose()` 轉交給 `ViewIme.Dispose()`
	- `ViewIme.Dispose()` 再向下釋放 `ViewKeyBoard`、`VmIme` 與各子模塊

	目前已顯式解除的訂閱包括：
	- `VmIme`：解除 `PropertyChanged` 與 `ImeState.AfterInput`
	- `VmCandidatesBar`：解除 `ImeState.AfterInput`
	- `VmInput`：解除 `ImeState.AfterInput`
	- `VmClipboard` / `VmToolBar` / `VmKey`：解除各自對宿主或全局狀態的訂閱
	- `ViewKeyBoard`：回收所有 `VmKey`，並解除對 `VmKeyBoard.PropertyChanged` 的監聽

	這一層文檔的約束是：
	- 以後若再新增掛在 `ImeState` / `RimeConnectionState` / 其他長生命週期單例上的事件訂閱，必須同步補 `Dispose`
	- 否則 IME 每次 hide/show 重建 UI 後，舊樹仍會收到事件，最終表現爲越來越卡、甚至崩潰
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
	- 鎖定開啟時，`$` 鍵背景色切換為 `UiCfg.MainColor`

	M 鍵保留單擊 `m`；
	滑動時發送固定字串後再補一次 `Space` 上屏：
	- 下滑：`$m,` + `Space`
	- 左滑：`$m,j` + `Space`
	- 右滑：`$m,i` + `Space`
]

#H[UiCfg][
	`Avalime.UI.UiCfg` 統一保存 UI 主色配置。
	當前 `MainColor` 與按鍵按下態背景色保持一致；`$` 鍵的 Shift 鎖定高亮也引用這個值。
	鍵盤整體字體會讀 `KeysCfg.Keyboard.Font`。
	`Keyboard.Font.Path` 必須指向單個字體文件；`Keyboard.Font.Family` 可選。
	若 `Family` 有值，直接按 `file-uri#family` 的方式交給 Avalonia 解析。
	若 `Family` 爲空，則從字體文件的 name table 中讀取 family name，再按同樣方式解析。
	若配置值爲空、字體文件不存在、family 無法解析，則回退到宿主原本默認字體。
]

""")]
file class _{
}
