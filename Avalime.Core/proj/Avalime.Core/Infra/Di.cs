using Microsoft.Extensions.DependencyInjection;
using Tsinswreng.CsCore;
namespace Avalime.Core.Infra;

[Doc("全局依賴注入管理")]
public class Di{
	public static IServiceProvider SvcP{get;set;} = null!;

	public static T GetRSvc<T>()
		where T:class
	{
		return SvcP.GetRequiredService<T>();
	}
}
