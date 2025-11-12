using Avalime.Core.IF;
namespace Avalime.Core;


public class Result<T>
	: I_Result<T>
{
	public T? data{get;} = default;

	public bool ok{get;} = true;

	public IEnumerable<object?>? errors{get;}=null;
	public static Result<T> Ok = new Result<T>();
}
