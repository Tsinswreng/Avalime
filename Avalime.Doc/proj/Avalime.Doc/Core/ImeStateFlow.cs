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
	- `ImeKeyProcessor.OnKeyEventsAsy(...)`
	- `AfterInput`
]

#H[觀察點][
	- `VmInput` 用 `AfterInput` 讀 preedit
	- `VmCandidatesBar` 用 `AfterInput` 刷候選詞
]

""")]
file class _{
}
