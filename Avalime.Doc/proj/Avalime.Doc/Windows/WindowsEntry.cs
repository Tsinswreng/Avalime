namespace Avalime.Doc.Windows;

using Tsinswreng.CsCore;

[Doc($$"""
#H[用途][
	Windows 入口負責組裝 DI，然後啟動 Avalonia desktop lifetime。
]

#H[入口行為][
	`Program.Main()`：
	- 註冊 `RimeSetup.Inst`，讓桌面端 `SvcIme` 和 `RimeKeyProcessor` 共用同一個 Rime session
	- 註冊 `IOsKeyProcessor`
	- 註冊桌面宿主 `IKeyboardHost`、`IClipboardService`
	- 註冊 `IImeKeyProcessor -> RimeKeyProcessor`
	- 註冊 `ImeUiState`
	- 註冊 `ISvcIme -> Avalime.Rime.SvcIme`
	- `Di.SvcP = provider`
	- `StartWithClassicDesktopLifetime(...)`
]

""")]
file class _{
}
