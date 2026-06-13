using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using Avalime.Core.Keys;
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
	const string Tag = "Avalime";

	protected override void OnCreate(Bundle? savedInstanceState){
		Log.Info(Tag, "OnCreate");
		var svc = new ServiceCollection();
		svc.AddSingleton<IImeKeyProcessor, StubImeKeyProcessor>();
		svc.AddSingleton<I_OsKeyProcessor, StubOsKeyProcessor>();
		svc.AddSingleton<ImeState>();
		var sp = svc.BuildServiceProvider(new ServiceProviderOptions{ValidateOnBuild = false, ValidateScopes = false});
		App.SetSvcProvider(sp);
		Log.Info(Tag, "OnCreate DI done");
		base.OnCreate(savedInstanceState);
		Log.Info(Tag, "OnCreate done");
	}
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
