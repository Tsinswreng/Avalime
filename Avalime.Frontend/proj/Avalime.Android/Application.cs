using Android.App;
using Android.Runtime;
using Avalime.UI;
using Avalonia;
using Avalonia.Android;
using System.Diagnostics;
using System.IO;
namespace Avalime.Android;

[Application]
public class Application : AvaloniaAndroidApplication<App>
{
	public Application(nint javaReference, JniHandleOwnership transfer)
		: base(javaReference, transfer)
	{
		var soDir = SetupSoFiles(global::Android.App.Application.Context);
		Avalime.Rime.RimeSetup.dllPath = System.IO.Path.Combine(soDir, "librime.so");
		Avalime.Rime.RimeSetup.userDataDir = "/sdcard/rime";
	}

	static string SetupSoFiles(global::Android.Content.Context ctx)
	{
		var internalDir = ctx.FilesDir!.AbsolutePath!;
		var externalDir = "/sdcard/rime";
		string[] soFiles = ["libc++_shared.so", "libCsRimeLua.so", "librime.so"];

		foreach (var so in soFiles)
		{
			var dst = System.IO.Path.Combine(internalDir, so);
			// 優先：內部目錄已有就不複製（開發者用 run-as cp 放進去的）
			if (System.IO.File.Exists(dst))
			{
				System.Diagnostics.Debug.WriteLine("[Avalime] already in place: " + dst);
				continue;
			}
			// 嘗試從 /sdcard/rime/ 複製（需要儲存權限，現代 Android 通常失敗）
			var src = System.IO.Path.Combine(externalDir, so);
			try
			{
				if (System.IO.File.Exists(src))
				{
					System.IO.File.Copy(src, dst, overwrite: true);
					System.Diagnostics.Debug.WriteLine("[Avalime] copied " + src + " -> " + dst);
				}
			}
			catch (System.Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("[Avalime] copy skipped for " + so + ": " + ex.Message);
			}
		}

		// preload libc++_shared.so, required by librime.so
		var libcpp = System.IO.Path.Combine(internalDir, "libc++_shared.so");
		if (System.IO.File.Exists(libcpp))
		{
			try
			{
				System.Runtime.InteropServices.NativeLibrary.Load(libcpp);
				System.Diagnostics.Debug.WriteLine("[Avalime] preloaded libc++_shared.so");
			}
			catch (System.Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("[Avalime] preload libc++_shared.so failed: " + ex.Message);
			}
		}

		return internalDir;
	}

	protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
	{
		return base.CustomizeAppBuilder(builder)
			.With(new Avalonia.Media.FontManagerOptions{
				DefaultFamilyName = "serif"
			});
	}
}
