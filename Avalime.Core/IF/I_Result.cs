namespace Avalime.Core.IF;
public interface I_Result<T>{
	public T? data{get;}
	public bool ok{get;}
	public IEnumerable<object?>? errors{get;}
}
