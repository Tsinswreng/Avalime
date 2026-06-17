namespace Avalime.Doc.Core;

using Tsinswreng.CsCore;

[Doc($$"""
#H[用途][
	`ImeState` 是整個輸入鏈的狀態樞紐。
	它接收按鍵事件，按順序分發給 `IImeKeyProcessor`，再通知 `AfterInput`。
]

#H[核心流程][
	- `Input(IEnumerable<IKeyEvent>)`
	- `BeforeInput`
	- `ImeKeyProcessor.OnKeyEventsAsy(...)` → 返回 `RespOnKeyEvent`（含 `Commits` 列表）
	- `AfterInput`
	- 若有 commit 文字，逐一觸發 `OnCommit`
]

#H[Commit 機制][
	`OnCommit` 事件在 `AfterInput` 之後觸發。
	它接收 Rime 引擎 commit 的文字（由 `RimeKeyProcessor` 通過 `get_commit` 取得），
	並通知平台層將文字輸出到 OS 輸入框。
	每個 commit 文字會單獨觸發一次事件。
]

#H[觀察點][
	- `VmInput` 用 `AfterInput` 讀 preedit
	- `VmCandidatesBar` 用 `AfterInput` 刷候選詞
	- Android `AvalimeInputMethodService` 訂閱 `OnCommit`，通過 `CommitText` 輸出到目標 App
]

""")]
file class _{
}
