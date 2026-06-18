namespace Avalime.Doc.Rime;

using Tsinswreng.CsCore;

[Doc($$"""
#H[用途][
	`RimeSetup` 負責載入 `librime.so`、初始化 traits、建立 session。
	`RimeKeyProcessor` 負責把按鍵轉成 Rime key code 並送入 session。
]

#H[Android 初始化][
	Android 端在 `Application` 啟動時設置：
	- `RimeSetup.dllPath`
	- `RimeSetup.userDataDir`
	- 之後再由 `RimeSetup.Inst` 完成初始化
]

#H[按鍵轉換][
	`RimeKeyCharConverter` 負責把 `IKeyEvent` 轉成 Rime 的 keycode / mask。
]

#H[Commit 檢測][
	`RimeKeyProcessor.OnKeyEventsAsy` 在每個 `process_key` 之後調用 `get_commit`。
	若有 commit 文字，收集到 `RespOnKeyEvent.Commits` 中返回給 `ImeState`，然後調用 `free_commit` 釋放。
]

#H[Option 通知][
	`RimeSetup.on_message` 解析 Rime 的 `option` 通知（如 `ascii_mode` / `!ascii_mode`），
	通過靜態事件 `OnOptionChanged` 通知訂閱者。
	`RimeConnectionState` 訂閱此事件來維護 `IsAsciiMode` 屬性。
]

#H[未處理按鍵檢測][
	`process_key` 返回值表示 Rime 是否處理了該按鍵。
	若返回 `False`，按鍵被收集到 `RespOnKeyEvent.UnhandledKeys`。
	此方法為同步方法（`Task.FromResult`），不在 async 狀態機中取 stack 變數地址，避免原生崩潰。
]

""")]
file class _{
}
