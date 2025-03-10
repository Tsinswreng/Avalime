
using Avalime.Core.IF;

namespace Avalime.Core.keys;

public class ImeState
	//:I_ImeState //TODO 先叶後抽象
{

	public IDictionary<object, object?> config{get;set;}= new Dictionary<object, object?>();
	public I_OsKeyProcessor osKeyProcessor{get;set;}
	public I_ImeKeyProcessor imeKeyProcessor{get;set;}

	public ImeState(I_OsKeyProcessor osKeyProcessor, I_ImeKeyProcessor imeKeyProcessor) {
		this.osKeyProcessor = osKeyProcessor;
		this.imeKeyProcessor = imeKeyProcessor;
	}

	public ImeState(I_OsKeyProcessor osKeyProcessor) {
		this.osKeyProcessor = osKeyProcessor;
	}

	public I_Result<object?> input(IEnumerable<I_KeyEvent> keyEvents){
		System.Console.WriteLine(onInput==null);
		onInput?.Invoke(this, keyEvents);

		return new Result<object?>();
	}

	public event EventHandler<IEnumerable<I_KeyEvent>> onInput;


	public object? getOption() {
		throw new NotImplementedException();
	}

	public object setOption() {
		throw new NotImplementedException();
	}
}