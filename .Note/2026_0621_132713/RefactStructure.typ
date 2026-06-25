2026_0621_132727[
看 E:\_code\CsRime\.agents\skills\avalime-doc\SKILL.typ

E:\_code\CsRime\Avalime\Avalime.Core\proj\Avalime.Core\Keys\SvcIme.cs

现在要求Avalime前端不能直接依赖引擎 必须依赖
Core抽象層


然後到安卓程序集入口中build一下 直到無錯誤。

]



2026_0625_160637[
````cs
File: e:\_code\CsRime\Avalime\Avalime.Core\proj\Avalime.Core\Keys\SvcIme.cs
144: 	public virtual Task ConnectAsy(CT ct = default){
145: 		IsConnected = ImeKeyProcessor is not null;
146: 		if(IsConnected && string.IsNullOrWhiteSpace(StatusText)){
147: 			StatusText = "Ime 已連接";
148: 		}
149: 		return Task.CompletedTask;
150: 	}

````

這東西好完全不應該出現在SvcIme中 違反架構設計規範 不是嗎
然後當前SvcIme中 候選詞的獲取也是設計得有問題的
明明底層引擎用的是迭代器來獲取
但在SvcIme中卻用的是List
我覺得應該統配成 `IAsyncEnumerable<ICandidate>`
然後

]
