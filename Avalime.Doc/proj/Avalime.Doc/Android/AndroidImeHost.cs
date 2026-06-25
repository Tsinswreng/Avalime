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
	輸入法服務顯式使用 `MyTheme.Ime`，避免沿用 Activity 的透明窗口配置。
	隱藏鍵盤時仍只調 `RequestHideSelf(0)`；
	但因爲 Android 13 上復用同一棵 `AvaloniaView` 在再次顯示後容易出現「可點擊但整體發黑」的黑屏，
	現在會在 `OnFinishInputView()` 把 `_shouldRecreateInputView` 置爲 `true`，
	並在下次 `OnStartInputView()` 時通過 `SetInputView(OnCreateInputView())` 重建整個 `AvaloniaView` 宿主。
	`OnCreateInputView()` 重建前還會先嘗試 `Dispose` 舊 `Content`，避免舊 UI 樹殘留訂閱。
]

#H[默認字體][
	`Application.CustomizeAppBuilder` 中設置 `DefaultFamilyName = "serif"`（帶襯線字體）。
	替代了原來的 `WithInterFont()`（無襯線）。
]

#H[文字輸出][
	`OnCreate` 中設定 `ImeState.OsKeyProcessor` 為 `AndroidOsKeyProcessor`，
	並訂閱 `ImeState.OnCommit` 事件。
	- `OnCommit`：收到 commit 文字後，通過 `CurrentInputConnection.CommitText` 輸出。
	- `AndroidOsKeyProcessor`：將 Rime 未處理的按鍵轉發給 OS（Backspace→`Delete`、Enter→`Enter`、普通文字鍵→`CommitText`）。
	- 若按鍵事件攜帶 `KeyBoardState` 中的 Ctrl/Shift/Alt/Meta 狀態，則改為 `SendKeyEvent` 發送帶 meta state 的系統組合鍵（如 Ctrl+A）。
	若當前沒有 `InputConnection`，則跳過輸出並記錄日誌。

	由於按鍵輸入現在通過 `ImeState.InputSafely(...)` 在後台執行，
	`OnCommit` 回調裡需要顯式切回 Android 主線程，再調 `CurrentInputConnection.CommitText`。
]

#H[依賴注入與日誌][
	Android 端在 `InputMethodService.OnCreate()` 組裝 `ServiceCollection`，並寫入全局 `Di.SvcProvider`。
	`Di` 是當前全局 DI 中心；UI / Core 側若拿服務，統一通過 `Di.GetRSvc<T>()`。
	`ISvcIme` 在 Android 端不再直接使用空殼基類，而是註冊 `AndroidRimeImeService`。
	這樣 `Avalime.UI` 仍只依賴 `Avalime.Core`，但真正的 Rime 連接、狀態同步與 option 切換由 Android 入口層實現。
	`ILogger` 也註冊爲 `AppLog.Inst`，方便不便注入的地方與可注入的地方共用同一條日誌出口。
	`AppLog.Inst.InnerLogger` 在 Android `Application` 啟動時切到 `AndroidLogger`，Release 也可直接進 logcat。
]

#H[原生庫載入][
	Android 端不再把 `librime.so` 放進 APK 的 `lib/arm64-v8a/` 標準原生庫路徑。
	原因是某些 `librime_jni.so` 變體在 `JNI_OnLoad` 會主動查找宿主 App 裡的 Java 類，
	如果被系統在應用啟動期自動 `loadLibrary`，會過早執行 JNI 初始化並直接崩潰。

	現在做法是：
	- 將原生庫內容以非 `.so` 文件名（如 `librime.bin`）作為 `AndroidAsset` 打包進 APK
	- `Avalime.Ro.jsonc` 放在 App 私有目錄，只保存 `RwCfgPath`
	- 若 `/sdcard/Android/.../files/Avalime.Rw.jsonc` 不存在，則從 APK 資產複製一份初始配置
	- 啟動時解包 `librime.bin` 到 App 私有目錄並還原成 `librime.so`
	- 再把實際 `DllPath`、`user_data_dir`、`app_name` 寫入雙源配置的可寫層，供 `RimeSetup` 按配置讀取
]

""")]
file class _{
}
