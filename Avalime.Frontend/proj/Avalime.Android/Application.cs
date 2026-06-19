using Android.App;
using Android.Runtime;
using Avalime.Core.Infra;
using Avalime.UI;
using Avalonia;
using Avalonia.Android;
using Tsinswreng.CsCfg;
using Tsinswreng.CsTools;
using System.Diagnostics;
using System.IO;
using AssetManager = Android.Content.Res.AssetManager;
namespace Avalime.Android;

[Application]
public class Application : AvaloniaAndroidApplication<App>
{
	const string LocalRimeSoName = "librime.so";

	public Application(nint javaReference, JniHandleOwnership transfer)
		: base(javaReference, transfer)
	{
		InitCfg(global::Android.App.Application.Context);
		SetupSoFiles(global::Android.App.Application.Context);
	}

	static void InitCfg(global::Android.Content.Context ctx)
	{
		var internalDir = ctx.FilesDir!.AbsolutePath!;
		var externalDir = ctx.GetExternalFilesDir(null)?.AbsolutePath
			?? throw new InvalidOperationException("Cannot get ExternalFilesDir");
		EnsureDir(externalDir);

		var roCfgPath = Path.Combine(externalDir, "Avalime.Ro.jsonc");
		var internalRoSeedPath = Path.Combine(internalDir, "Avalime.Ro.jsonc");
		var internalRwSeedPath = Path.Combine(internalDir, "Avalime.Rw.jsonc");
		EnsureAssetFile(ctx.Assets!, "Avalime.Ro.jsonc", internalRoSeedPath, overwrite: true);
		EnsureAssetFile(ctx.Assets!, "Avalime.Rw.jsonc", internalRwSeedPath, overwrite: true);
		if(ShouldRefreshFromSeed(roCfgPath)){
			CopyFileToPath(internalRoSeedPath, roCfgPath, overwrite: true);
		}

		var dualSrcCfg = AppCfg.Inst;
		var roCfg = new JsonFileCfgAccessor();
		dualSrcCfg.RoCfg = roCfg;
		roCfg.FromFile(roCfgPath);

		var rwCfgPath = KeysCfg.RwCfgPath.GetFrom(dualSrcCfg) ?? "Avalime.Rw.jsonc";
		rwCfgPath = Path.IsPathRooted(rwCfgPath)
			? rwCfgPath
			: Path.Combine(externalDir, rwCfgPath);
		if(ShouldRefreshFromSeed(rwCfgPath)){
			CopyFileToPath(internalRwSeedPath, rwCfgPath, overwrite: true);
		}

		var rwCfg = new JsonFileCfgAccessor();
		dualSrcCfg.RwCfg = rwCfg;
		rwCfg.FromFile(rwCfgPath);
	}

	static string SetupSoFiles(global::Android.Content.Context ctx)
	{
		var internalDir = ctx.FilesDir!.AbsolutePath!;
		var externalDir = ctx.GetExternalFilesDir(null)?.AbsolutePath
			?? throw new InvalidOperationException("Cannot get ExternalFilesDir");
		EnsureDir(externalDir);
		string[] passthroughSoFiles = ["libc++_shared.so", "libCsRimeLua.so"];

		foreach (var so in passthroughSoFiles)
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

		var localRime = System.IO.Path.Combine(internalDir, LocalRimeSoName);
		ToolFile.EnsureFile(localRime);
		try
		{
			ExtractAssetFile(ctx.Assets!, "rime/librime.bin", localRime);
			System.Diagnostics.Debug.WriteLine("[Avalime] extracted asset rime/librime.bin -> " + localRime);
		}
		catch (System.Exception ex)
		{
			System.Diagnostics.Debug.WriteLine("[Avalime] extract asset rime failed: " + ex.Message);
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

		var userDataDir = KeysCfg.Librime.RimeTraits.user_data_dir.GetFrom(AppCfg.Inst)
			?? Path.Combine(externalDir, "UserData");
		EnsureDir(userDataDir);
		AppCfg.Inst.Set(KeysCfg.Librime.DllPath, localRime);
		AppCfg.Inst.Set(KeysCfg.Librime.RimeTraits.user_data_dir, userDataDir);
		AppCfg.Inst.Set(KeysCfg.Librime.RimeTraits.app_name, ctx.PackageName ?? "rime.avalime");

		return internalDir;
	}

	static void EnsureAssetFile(AssetManager assets, string assetPath, string outputPath, bool overwrite)
	{
		EnsureParentDir(outputPath);
		if(File.Exists(outputPath) && !overwrite){
			return;
		}
		ExtractAssetFile(assets, assetPath, outputPath);
	}

	static void EnsureDir(string dirPath)
	{
		if(string.IsNullOrWhiteSpace(dirPath)){
			return;
		}
		var keepFile = Path.Combine(dirPath, ".keep");
		ToolFile.EnsureFile(keepFile);
		if(File.Exists(keepFile)){
			File.Delete(keepFile);
		}
	}

	static void ExtractAssetFile(AssetManager assets, string assetPath, string outputPath)
	{
		using var input = assets.Open(assetPath);
		using var output = System.IO.File.Create(outputPath);
		input.CopyTo(output);
	}

	static void CopyFileToPath(string srcPath, string dstPath, bool overwrite)
	{
		EnsureParentDir(dstPath);
		if(File.Exists(dstPath) && !overwrite){
			return;
		}
		File.Copy(srcPath, dstPath, overwrite: true);
	}

	static bool ShouldRefreshFromSeed(string path)
	{
		if(!File.Exists(path)){
			return true;
		}
		var fi = new FileInfo(path);
		return fi.Length == 0;
	}

	static void EnsureParentDir(string filePath)
	{
		if(string.IsNullOrWhiteSpace(filePath)){
			return;
		}
		var dir = Path.GetDirectoryName(filePath);
		if(string.IsNullOrWhiteSpace(dir)){
			return;
		}
		EnsureDir(dir);
	}

	protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
	{
		return base.CustomizeAppBuilder(builder)
			.With(new Avalonia.Media.FontManagerOptions{
				DefaultFamilyName = "serif"
			});
	}
}
