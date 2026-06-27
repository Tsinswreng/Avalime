using Android.App;
using Android.Content.PM;
using Android.OS;
using Avalonia.Android;

namespace Avalime.Android;

[Activity(
	Label = "Avalime",
	Theme = "@style/MyTheme.NoActionBar",
	Icon = "@drawable/icon",
	MainLauncher = true,
	ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity
{
	protected override void OnCreate(Bundle? savedInstanceState) {
		base.OnCreate(savedInstanceState);
		AvalimeRecoveryNotification.Ensure(this);
		AvalimeOverlayPermission.Ensure(this);
	}

	/// 參照 Ngaq 的做法，權限授予後立刻補發常駐恢復通知。
	public override void OnRequestPermissionsResult(
		int requestCode, string[]? permissions, Permission[]? grantResults
	){
		base.OnRequestPermissionsResult(
			requestCode,
			permissions ?? [],
			grantResults ?? []
		);
		if(!AvalimeRecoveryNotification.IsPermissionRequest(requestCode)){
			return;
		}
		if(grantResults is null || grantResults.Length == 0){
			return;
		}
		if(grantResults[0] != Permission.Granted){
			return;
		}
		AvalimeRecoveryNotification.Ensure(this);
	}
}
