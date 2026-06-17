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

""")]
file class _{
}
