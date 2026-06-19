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

#H[默認字體][
	`Application.CustomizeAppBuilder` 中設置 `DefaultFamilyName = "serif"`（帶襯線字體）。
	替代了原來的 `WithInterFont()`（無襯線）。
]

#H[文字輸出][
	`OnCreate` 中設定 `ImeState.OsKeyProcessor` 為 `AndroidOsKeyProcessor`，
	並訂閱 `ImeState.OnCommit` 事件。
	- `OnCommit`：收到 commit 文字後，通過 `CurrentInputConnection.CommitText` 輸出。
	- `AndroidOsKeyProcessor`：將 Rime 未處理的按鍵轉發給 OS（Backspace→`Delete`、Enter→`Enter`、文字鍵→`CommitText`）。
	若當前沒有 `InputConnection`，則跳過輸出並記錄日誌。

	由於按鍵輸入現在通過 `ImeState.InputSafely(...)` 在後台執行，
	`OnCommit` 回調裡需要顯式切回 Android 主線程，再調 `CurrentInputConnection.CommitText`。
]

#H[原生庫載入][
	Android 端不再把 `librime.so` 放進 APK 的 `lib/arm64-v8a/` 標準原生庫路徑。
	原因是某些 `librime_jni.so` 變體在 `JNI_OnLoad` 會主動查找宿主 App 裡的 Java 類，
	如果被系統在應用啟動期自動 `loadLibrary`，會過早執行 JNI 初始化並直接崩潰。

	現在做法是：
	- 將原生庫內容以非 `.so` 文件名（如 `librime.bin`）作為 `AndroidAsset` 打包進 APK
	- 啟動時解包到 App 私有目錄並還原成 `librime.so`
	- 再由 `RimeSetup.dllPath` 指向該私有文件路徑，按需載入
]

""")]
file class _{
}
