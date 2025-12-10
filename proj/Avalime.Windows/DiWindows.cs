using Avalime.Core.Keys;
using Avalime.Rime;
using Microsoft.Extensions.DependencyInjection;

namespace Avalime.Windows;

public static class DiWindows{
	extension(ServiceCollection z){
		public ServiceCollection SetupWindows(){
z.AddSingleton<
	I_OsKeyProcessor
	, WindowsKeyProcessor
>();

z.AddSingleton<
	IImeKeyProcessor
	, RimeKeyProcessor
>();

z.AddSingleton<
	ImeState
>();
return z;
		}

	}
}
