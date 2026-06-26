namespace Avalime.Windows;

using System;
using System.IO;
using Avalime.Core.Infra;
using Avalime.Core.Infra.Log;
using Avalime.Core.Keys;
using Avalime.Rime;
using Avalime.UI;
using Avalime.UI.Views.CandidatesBar;
using Avalime.UI.Views.Clipboard;
using Avalime.UI.Views.Ime;
using Avalime.UI.Views.Input;
using Avalime.UI.Views.Key;
using Avalime.UI.Views.KeyBoard;
using Avalime.UI.Views.RimeLog;
using Avalime.UI.Views.ToolBar;
using Avalonia;
using Avalonia.Media;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Tsinswreng.CsCfg;


sealed class Program{
	const str RimeDllPath = @"D:\ENV\Rime\weasel-0.15.0\rime.dll";
	const str UserDataDir = @"D:\Program Files\Rime\User_Data";

	[STAThread]
	public static void Main(string[] args){
		AppLog.Inst.InnerLogger = NullLogger.Instance;
		InitCfg();
		SetupWindowsCfg();
		var services = new ServiceCollection();
		// Windows 桌面端也直接走 Rime 真實實現，避免 UI 啟動後再落回無法工作的 stub 組合。
		services.AddSingleton<RimeSetup>(_ => RimeSetup.Inst);
		services.AddSingleton<
			IOsKeyProcessor
			, WindowsKeyProcessor
		>();
		services.AddSingleton<IImeKeyProcessor, RimeKeyProcessor>();
		services.AddSingleton<IKeyboardHost, StubKeyboardHost>();
		services.AddSingleton<IClipboardService, StubClipboardService>();
		services.AddSingleton<ImeUiState>();
		services.AddSingleton<ILogger>(_ => AppLog.Inst);

		// SvcIme 依賴 RimeSetup 與 IImeKeyProcessor；兩者都要在 desktop 入口顯式註冊。
		services.AddSingleton<
			ISvcIme
				, SvcIme
		>();

		services.AddSingleton<RimeLogBuffer>();
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

	/// <summary>
	/// 按 Android 入口同樣的雙源配置流程初始化 Windows 端配置。
	/// Ro 只負責給出 Rw 配置路徑，Rw 承載實際可寫配置內容。
	/// </summary>
	static void InitCfg(){
		var baseDir = AppContext.BaseDirectory;
		var roCfgPath = Path.Combine(baseDir, "Avalime.Ro.jsonc");
		var dfltRwCfgPath = Path.Combine(baseDir, "Avalime.Rw.jsonc");

		if(!File.Exists(roCfgPath)){
			throw new FileNotFoundException("Windows ro config not found", roCfgPath);
		}

		var dualSrcCfg = AppCfg.Inst;
		var roCfg = new JsonFileCfgAccessor();
		dualSrcCfg.RoCfg = roCfg;
		roCfg.FromFile(roCfgPath);

		var rwCfgPath = KeysCfg.RwCfgPath.GetFrom(dualSrcCfg) ?? dfltRwCfgPath;
		if(!Path.IsPathRooted(rwCfgPath)){
			rwCfgPath = Path.Combine(baseDir, rwCfgPath);
		}
		if(!File.Exists(rwCfgPath)){
			throw new FileNotFoundException("Windows rw config not found", rwCfgPath);
		}

		var rwCfg = new JsonFileCfgAccessor();
		dualSrcCfg.RwCfg = rwCfg;
		rwCfg.FromFile(rwCfgPath);
	}

	/// <summary>
	/// Windows 端在啟動時把本機實際 Rime 路徑寫回可寫配置層，
	/// 保證後續 `RimeSetup` 與 UI 讀到的是當前機器可用路徑。
	/// </summary>
	static void SetupWindowsCfg(){
		AppCfg.Inst.Set(KeysCfg.Librime.DllPath, RimeDllPath);
		AppCfg.Inst.Set(KeysCfg.Librime.RimeTraits.user_data_dir, UserDataDir);
		AppCfg.Inst.Set(KeysCfg.Librime.RimeTraits.app_name, nameof(Avalime.Windows));
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
