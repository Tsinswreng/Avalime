using Avalime.Core.Keys;
using Microsoft.Extensions.DependencyInjection;

namespace Avalime.UI;

public static class AppServices
{
	static IServiceProvider? _svcP;

	public static IServiceProvider SvcP{
		get{
			if(_svcP is not null){
				return _svcP;
			}
			var svc = new ServiceCollection();
			svc.AddSingleton<IImeKeyProcessor, StubImeKeyProcessor>();
			svc.AddSingleton<I_OsKeyProcessor, StubOsKeyProcessor>();
			svc.AddSingleton<IKeyboardHost, StubKeyboardHost>();
			svc.AddSingleton<ImeState>();
			svc.AddSingleton<RimeConnectionState>();
			_svcP = svc.BuildServiceProvider(new ServiceProviderOptions{ValidateOnBuild = false, ValidateScopes = false});
			return _svcP;
		}
	}

	public static void SetSvcProvider(IServiceProvider svcP){
		_svcP = svcP;
	}

	public static T GetRequiredService<T>() where T : class
		=> SvcP.GetRequiredService<T>();
}

class StubOsKeyProcessor : I_OsKeyProcessor
{
	public event ErrHandler? OnErr;
	public Task<RespOnKeyEvent> OnKeyEventsAsy(IEnumerable<IKeyEvent> keyEvents)
		=> Task.FromResult(new RespOnKeyEvent());
}

class StubImeKeyProcessor : IImeKeyProcessor
{
	public event ErrHandler? OnErr;
	public Task<RespOnKeyEvent> OnKeyEventsAsy(IEnumerable<IKeyEvent> keyEvents)
		=> Task.FromResult(new RespOnKeyEvent());
}

class StubKeyboardHost : IKeyboardHost
{
	public void HideKeyboard(){}
}
