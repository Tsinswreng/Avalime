namespace Avalime.Windows;

using System;
using Avalime.Core.Infra;
using Avalime.Core.Infra.Log;
using Avalime.Core.Keys;
using Avalime.Rime;
using Avalime.UI;
using Avalime.UI.Views.ViewCandidatesBar;
using Avalime.UI.Views.ViewClipboard;
using Avalime.UI.Views.ViewIme;
using Avalime.UI.Views.ViewInput;
using Avalime.UI.Views.ViewKey;
using Avalime.UI.Views.ViewKeyBoard;
using Avalime.UI.Views.ViewRimeLog;
using Avalime.UI.Views.ViewToolBar;
using Avalonia;
using Avalonia.Media;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;


sealed class Program{
	[STAThread]
	public static void Main(string[] args){
		AppLog.Inst.InnerLogger = NullLogger.Instance;
		var services = new ServiceCollection();
		services.AddSingleton<
			I_OsKeyProcessor
			, WindowsKeyProcessor
		>();
		services.AddSingleton<IImeKeyProcessor, StubImeKeyProcessor>();
		services.AddSingleton<IKeyboardHost, StubKeyboardHost>();
		services.AddSingleton<IClipboardService, StubClipboardService>();
		services.AddSingleton<ImeUiState>();
		services.AddSingleton<ILogger>(_ => AppLog.Inst);

		services.AddSingleton<
			ImeState
		>();

		services.AddSingleton<RimeConnectionState>();
		services.AddTransient<VmIme>();
		services.AddTransient<VmToolBar>();
		services.AddTransient<VmCandidatesBar>();
		services.AddTransient<VmInput>();
		services.AddTransient<VmClipboard>();
		services.AddTransient<VmRimeLog>();
		services.AddTransient<VmKey>();
		services.AddTransient<VmKeyBoard>();

		var provider = services.BuildServiceProvider();
		BuildAvaloniaApp()
		.AfterSetup(e=>Di.SvcProvider = provider)
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
