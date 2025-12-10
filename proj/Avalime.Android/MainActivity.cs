using Android.App;
using Android.Content.PM;
using Android.InputMethodServices;
using Avalime.UI;
using Avalonia;
using Avalonia.Android;
namespace Avalime.Android;

[Activity(
	Label = "Avalime.Android",
	Theme = "@style/MyTheme.NoActionBar",
	Icon = "@drawable/icon",
	MainLauncher = true,
	ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity<App> {
	protected override AppBuilder CustomizeAppBuilder(AppBuilder builder) {
		return base.CustomizeAppBuilder(builder)
			.WithInterFont();
	}
}

class MyIme : InputMethodService{

}
