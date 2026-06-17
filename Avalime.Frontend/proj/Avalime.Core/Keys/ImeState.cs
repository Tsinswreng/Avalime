namespace Avalime.Core.Keys;

public class ImeState
	//:I_ImeState //TODO 先叶後抽象
{

	public IDictionary<object, object?> Cfg{get;set;}= new Dictionary<object, object?>();
	public I_OsKeyProcessor OsKeyProcessor{get;set;}
	public IImeKeyProcessor ImeKeyProcessor{get;set;}

	public ImeState(I_OsKeyProcessor osKeyProcessor, IImeKeyProcessor imeKeyProcessor) {
		this.OsKeyProcessor = osKeyProcessor;
		this.ImeKeyProcessor = imeKeyProcessor;
	}

	public ImeState(I_OsKeyProcessor osKeyProcessor) {
		this.OsKeyProcessor = osKeyProcessor;
	}


	public async Task<RespInput> Input(IEnumerable<IKeyEvent> keyEvents){
		BeforeInput?.Invoke(this, keyEvents);
		RespOnKeyEvent? resp = null;
		if(ImeKeyProcessor is not null){
			resp = await ImeKeyProcessor.OnKeyEventsAsy(keyEvents);
		}
		AfterInput?.Invoke(this, keyEvents);

		// 觸發 commit 事件
		if(resp?.Commits is not null){
			foreach(var commitText in resp.Commits){
				OnCommit?.Invoke(this, commitText);
			}
		}

		return new();
	}

	public event EventHandler<IEnumerable<IKeyEvent>> BeforeInput;
	public event EventHandler<IEnumerable<IKeyEvent>> AfterInput;

	/// <summary>
	/// 當 Rime 引擎 commit 文字時觸發。參數為 commit 的文字內容。
	/// </summary>
	public event EventHandler<string>? OnCommit;


	public object? GetOption() {
		throw new NotImplementedException();
	}

	public object SetOption() {
		throw new NotImplementedException();
	}
}
