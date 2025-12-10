using Avalime.Core.IF;

namespace Avalime.Core.Keys;

public interface IImeState{

	public IDictionary<object, object?> Cfg{get;set;}
	public object? GetOption();
	public object SetOption();
	public I_OsKeyProcessor OsKeyProcessor{get;set;}
	public IImeKeyProcessor ImeKeyProcessor{get;set;}

	/// <summary>
	/// test
	/// </summary>
	/// <param name="keyChars"></param>
	/// <returns></returns>
	public I_Result<object?> Input(IEnumerable<IKeyChar> keyChars);

	public event EventHandler<object?>? OnInput;


}
