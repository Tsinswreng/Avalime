using Avalime.Core.Keys;
using Avalime.Rime;
using Avalime.UI;
using Microsoft.Extensions.DependencyInjection;

namespace Avalime.Windows;

public static class DiWindows{
	extension(ServiceCollection z){
		public ServiceCollection SetupWindows(){
	z.AddSingleton<
		IOsKeyProcessor
		, WindowsKeyProcessor
	>();

	z.AddSingleton<
		IImeKeyProcessor
		, RimeKeyProcessor
	>();

	z.AddSingleton<
		ISvcIme
		, SvcIme
	>();

	z.AddSingleton<RimeLogBuffer>();
	return z;
		}

	}
}
