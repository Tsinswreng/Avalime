using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using Avalime.Core.Keys;
using Avalime.Rime;
using Avalime.UI;
using Avalonia.Android;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Avalime.Android;

[Activity(
	Label = "Avalime.Android",
	Theme = "@style/MyTheme.NoActionBar",
	Icon = "@drawable/icon",
	MainLauncher = true,
	ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity
{
	const string Tag = "Avalime.Android";

	protected override void OnCreate(Bundle? savedInstanceState){
		Log.Info(Tag, "OnCreate start");
		try{
			Init();
		}catch(Exception ex){
			Log.Error(Tag, $"Init failed: {ex}");
			throw;
		}
		Log.Info(Tag, "OnCreate calling base");
		base.OnCreate(savedInstanceState);
		Log.Info(Tag, "OnCreate done");
	}

	/// 初始化 DI 及 Rime、在 base.OnCreate (即 Avalonia 啟動) 之前執行
	void Init(){
		Log.Info(Tag, "Init start");

		// 1. 初始化 Rime
		try{
			Log.Info(Tag, $"Loading Rime: {RimeSetup.dllPath}");
			var rimeSetup = RimeSetup.Inst;
			Log.Info(Tag, $"Rime ok, session={rimeSetup.rimeSessionId}");
		}catch(Exception ex){
			Log.Error(Tag, $"Rime init failed: {ex}");
		}

		// 2. 建立 DI 容器
		var svc = new ServiceCollection();
		try{
			svc.AddSingleton<IImeKeyProcessor, RimeKeyProcessor>();
			svc.AddSingleton<I_OsKeyProcessor, StubOsKeyProcessor>();
			svc.AddSingleton<ImeState>();
		}catch(Exception ex){
			Log.Error(Tag, $"DI register failed: {ex}");
		}

		var sp = svc.BuildServiceProvider(new ServiceProviderOptions{
			ValidateOnBuild = false,
			ValidateScopes = false
		});
		App.SetSvcProvider(sp);
		Log.Info(Tag, "Init done");
	}
}

/// Android 端 OS KeyProcessor 空實現
class StubOsKeyProcessor : I_OsKeyProcessor
{
	public event ErrHandler? OnErr;

	public Task<RespOnKeyEvent> OnKeyEventsAsy(IEnumerable<IKeyEvent> keyEvents){
		return Task.FromResult(new RespOnKeyEvent());
	}
}
