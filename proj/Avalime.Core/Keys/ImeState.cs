
using System.Threading.Tasks;
using Avalime.Core.IF;

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


	public async Task<I_Result<object?>> Input(IEnumerable<IKeyEvent> keyEvents){
		BeforeInput?.Invoke(this, keyEvents);
		await ImeKeyProcessor.OnKeyEventsAsy(keyEvents);
		AfterInput?.Invoke(this, keyEvents);
		return new Result<object?>();
	}

	public event EventHandler<IEnumerable<IKeyEvent>> BeforeInput;
	public event EventHandler<IEnumerable<IKeyEvent>> AfterInput;


	public object? GetOption() {
		throw new NotImplementedException();
	}

	public object SetOption() {
		throw new NotImplementedException();
	}
}
