namespace Avalime.Doc.Windows;

using Tsinswreng.CsCore;

[Doc($$"""
#H[用途][
	Windows 入口負責組裝 DI，然後啟動 Avalonia desktop lifetime。
]

#H[入口行為][
	`Program.Main()`：
	- 註冊 `I_OsKeyProcessor`
	- 註冊 `ImeState`
	- 註冊 `RimeConnectionState`
	- `App.SetSvcProvider(...)`
	- `StartWithClassicDesktopLifetime(...)`
]

""")]
file class _{
}
