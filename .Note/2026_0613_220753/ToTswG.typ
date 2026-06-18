2026_0613_220817[
先看代碼規範
````
    - E:\_code\CsNgaq\.agents\skills\write-csharp-code\SKILL.md
    - E:\_code\CsNgaq\Doc\Spec\Common.typ
    - E:\_code\CsNgaq\Doc\Spec\Frontend.typ
    - E:\_code\CsNgaq\Ngaq.Frontend\proj\Ngaq.Ui\CodeTemplate\Sample\**
    - E:\_code\CsNgaq\Ngaq.Frontend\proj\Ngaq.Ui\Infra\ViewModelBase.cs
    - E:\_code\CsNgaq\Ngaq.Frontend\proj\Ngaq.Ui\Views\MainView.Decl.cs
    - E:\_code\CsNgaq\Doc\Spec\IView.typ
````

````yaml
File: d:\Program Files\Rime\User_Data\TswG-3.3.10.trime.yaml
1100:
1101: #键盘布局
1102: preset_keyboards:
1103:   default:
1104:     author: "Tsinswreng"
1105:     name: TswG
1106:     ascii_mode: 0
1107:     width: *TswG_width
1108:     height: 30

````

看這個文件
這個文件非常大
只挑你要看的部分來看 其他地方別看

重點看 TswG 的鍵盤佈局。

然後你把

E:\_code\CsRime\Avalime\proj\Avalime.UI\Views\KeyBoard\ViewKeyBoard.cs

的佈局和功能也改成和他的一樣。


]


2026_0618_185004[
键盘长按弄了吗?? 自己看 yaml裏面long click的部分

]


2026_0618_190003[
ascii_mode 也弄一下。
Y 鍵 向左滑動是切換ascii模式與非ascii模式

ascii 模式下 鍵盤所有按鍵都顯示爲小寫拉丁字母
比如 A ∑ 就 顯示成a s
ascii模式是引擎的概念 不是純前端的概念

]


2026_0618_193031[
我还没打字呢 我就连接rime之后 滑了几次y键 就闪退了 看看甚么原因。还有 你只用把字母改成小写字母 别的甚么都不用动 我看你连方向键 回车 退格 啥的显示都变了 不要这样
]
