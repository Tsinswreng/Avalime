namespace Avalime.Doc.Android;

using Tsinswreng.CsCore;

[Doc($$"""
#H[用途][
	Android 端是標準 `InputMethodService` 宿主。
	它把 Avalonia 視圖包進 `AvaloniaView`，並回傳給系統輸入法窗口。
]

#H[註冊][
	輸入法由：
	- `[Service]`
	- `android.view.InputMethod`
	- `@xml/ime_method`
	組成。
]

#H[窗口行為][
	`OnEvaluateFullscreenMode()` 會關掉全屏提取。
	`OnConfigureWindow()` 把高度限制成半屏。
]

#H[文字輸出][
	`OnCreate` 中訂閱 `ImeState.OnCommit` 事件。
	收到 commit 文字後，通過 `CurrentInputConnection.CommitText` 將文字輸出到目標 App 的輸入框。
	若當前沒有 `InputConnection`（輸入法未連接目標），則跳過輸出並記錄日誌。
]

""")]
file class _{
}
