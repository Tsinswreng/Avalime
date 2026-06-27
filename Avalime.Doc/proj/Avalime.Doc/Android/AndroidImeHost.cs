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
	IME 宿主現在改爲仿照 `AvlnImeDemo` 的方案：
	- `OnCreateInputView()` 只創建一次 `LoggingAvaloniaView`
	- 後續 hide/show 都複用同一個 view，不再反覆重建整棵 Avalonia UI
	- 在 `OnCreateInputView()` 與 `OnConfigurationChanged()` 前，宿主都會先把複用 view 從舊父節點摘下；這是爲了避免橫豎屏切換後 `InputMethodService` 內部重新 `setInputView(...)` 時因“child already has a parent”而崩潰
	- `OnStartInputView()` 會先把該 view 從舊父節點摘下，再 `SetInputView(...)` 重掛回窗口
	- `OnWindowShown()` 只補 `RequestLayout()` / `Invalidate()` 的輕量刷新，不再二次重掛；這是爲了減少橫豎屏切換後首次彈出時的額外 surface 重建成本
	- `Application` 與 `InputMethodService.OnCreate()` 都會觸發一次後台 `RimeWarmup`，把 librime 初始化儘量前移到“鍵盤真正顯示之前”
	- 另外會啓動一個常駐前台通知；若偶發黑屏且無法自行恢復，用戶可點通知觸發“強制恢復”
	- “強制恢復”路徑不改動默認 hide 行爲，而是丟棄當前 `LoggingAvaloniaView`、重建新的 `MainView` 與 `AvaloniaView`，再重掛回 IME window
	- Android 9+ 還會額外調 `RequestShowSelf(ShowFlags.Forced)`，嘗試把重建後的鍵盤重新拉回可見狀態

	這條路徑的出發點是：
	相比“每次 hide 後強制重建整棵 view”，單例複用 + 重掛更接近系統原生輸入法窗口的生命週期，
	也更符合已驗證過的 `AvlnImeDemo` 行爲。

	另外，Android 端現在加入了分體懸浮鍵盤模式：
	- 用戶在工具欄手動點擊分體按鈕後，狀態持久化保存
	- 若開啓分體，IME 主窗口不再承載整塊鍵盤，而是縮成極薄佔位 view
	- 左右兩半鍵盤與中間窄頂條改走 `TYPE_APPLICATION_OVERLAY`
	- 這樣中間 50% 區域不屬於 IME 視窗，可把點擊交回底下 App
	- 缺少 overlay 權限時，宿主會跳轉系統設置頁，並回退到普通整體鍵盤，避免進入不可用狀態
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
	`ISvcIme` 在 Android 端不再直接使用空殼基類，而是註冊 `Avalime.Rime.SvcIme`。
	這樣 `Avalime.UI` 仍只依賴 `Avalime.Core`，但真正的 Rime 連接、狀態同步與 option 切換由 `Avalime.Rime` 層實現。
	`RimeWarmup` 也是 Android 宿主主動觸發的：目的不是立刻把鍵盤 UI 綁死在後端初始化上，而是儘量利用進程啓動與 IME service 建立的空檔，在後台先把 `RimeSetup.Inst` 單例拉起來。
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
