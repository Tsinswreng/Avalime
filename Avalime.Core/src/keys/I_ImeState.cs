using Avalime.Core.IF;

namespace Avalime.Core.keys;

public interface I_ImeState{

	public IDictionary<object, object?> config{get;set;}
	public object? getOption();
	public object setOption();
	public I_OsKeyProcessor osKeyProcessor{get;set;}
	public I_ImeKeyProcessor imeKeyProcessor{get;set;}

	/// <summary>
	/// test
	/// </summary>
	/// <param name="keyChars"></param>
	/// <returns></returns>
	public I_Result<object?> input(IEnumerable<I_KeyChar> keyChars);

}
