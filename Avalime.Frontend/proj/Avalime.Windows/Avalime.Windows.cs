namespace Avalime.Windows;

using System;
using Avalime.Core.Keys;
using Avalime.Rime;
using Avalime.UI;
using Avalonia;
using Avalonia.Media;
using Microsoft.Extensions.DependencyInjection;


sealed class Program{
	[STAThread]
	public static void Main(string[] args){
		var services = new ServiceCollection();
		services.AddSingleton<
			I_OsKeyProcessor
			, WindowsKeyProcessor
		>();

		// IImeKeyProcessor NOT registered — Connect button switches it to RimeKeyProcessor
		// (same flow as Android, avoids auto-init before .so is ready)

		services.AddSingleton<
			ImeState
		>();

		services.AddSingleton<RimeConnectionState>();

		var provider = services.BuildServiceProvider();
		BuildAvaloniaApp()
		.AfterSetup(e=>App.SetSvcProvider(provider))
		.StartWithClassicDesktopLifetime(args);
	}

	public static AppBuilder BuildAvaloniaApp(){
		var options = new FontManagerOptions {
		DefaultFamilyName = "孤鹜 筑紫明朝",
		FontFallbacks = new[] { new FontFallback { FontFamily = "Microsoft YaHei" } }
	};
		return AppBuilder.Configure<App>()
			.UsePlatformDetect()
			.With(options)
			.LogToTrace();
	}

}
