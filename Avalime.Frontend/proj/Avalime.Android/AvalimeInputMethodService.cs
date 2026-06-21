using Android.App;
using Android.Content;
using Android.InputMethodServices;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Avalime.Core.Infra;
using Avalime.Core.Infra.Log;
using Avalime.Core.Keys;
using Avalime.UI;
using Avalime.UI.Views;
using Avalime.UI.Views.CandidatesBar;
using Avalime.UI.Views.Clipboard;
using Avalime.UI.Views.Ime;
using Avalime.UI.Views.Input;
using Avalime.UI.Views.Key;
using Avalime.UI.Views.KeyBoard;
using Avalime.UI.Views.RimeLog;
using Avalime.UI.Views.ToolBar;
using Avalonia.Android;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Handler = Android.OS.Handler;
using Looper = Android.OS.Looper;

namespace Avalime.Android;

[Service(
	Label = "Avalime",
	Permission = global::Android.Manifest.Permission.BindInputMethod,
	Exported = true)]
[IntentFilter(new[] { "android.view.InputMethod" })]
[MetaData("android.view.im", Resource = "@xml/ime_method")]
public class AvalimeInputMethodService : InputMethodService {
	public AvalimeInputMethodService() { }
	public AvalimeInputMethodService(nint javaReference, JniHandleOwnership transfer)
		: base(javaReference, transfer) { }

	LoggingAvaloniaView? _inputView;
	bool _shouldRecreateInputView;

	int GetHalfScreenHeight() {
		var screenHeight = Resources?.DisplayMetrics?.HeightPixels ?? 0;
		return screenHeight > 0 ? screenHeight / 2 : ViewGroup.LayoutParams.WrapContent;
	}

	public override bool OnEvaluateFullscreenMode() {
		return false;
	}

	public override void OnConfigureWindow(Window? win, bool isFullscreen, bool isCandidatesOnly) {
		base.OnConfigureWindow(win, false, isCandidatesOnly);
		win?.SetLayout(ViewGroup.LayoutParams.MatchParent, GetHalfScreenHeight());
	}

	public override global::Android.Views.View OnCreateInputView() {
		AppLog.Info("[IME] OnCreateInputView");

		if (_inputView != null && !_shouldRecreateInputView)
			return _inputView;

		if (_inputView?.Content is IDisposable disposableContent) {
			disposableContent.Dispose();
		}

		_inputView = new LoggingAvaloniaView(this) {
			Content = new MainView()
		};
		_inputView.LayoutParameters = new ViewGroup.LayoutParams(
			ViewGroup.LayoutParams.MatchParent,
			GetHalfScreenHeight());
		_shouldRecreateInputView = false;

		return _inputView;
	}

	public void HideKeyboardAndRecreateInputViewOnNextShow() {
		AppLog.Info("[IME] HideKeyboard requested");
		RequestHideSelf(0);
	}

	public override void OnCreate() {
		SetTheme(Resource.Style.MyTheme_Ime);
		base.OnCreate();
		AppLog.Info("[IME] OnCreate");

		var services = new ServiceCollection();
		services.AddSingleton<IImeKeyProcessor, AndroidStubImeKeyProcessor>();
		services.AddSingleton<IOsKeyProcessor, AndroidStubOsKeyProcessor>();
		services.AddSingleton<IKeyboardHost>(_ => new AndroidKeyboardHost(() => this));
		services.AddSingleton<IClipboardService, AndroidClipboardService>();
		services.AddSingleton<ImeUiState>();
		services.AddSingleton<ImeState>();
		services.AddSingleton<RimeConnectionState>();
		services.AddSingleton<RimeLogBuffer>();
		services.AddTransient<VmIme>();
		services.AddTransient<VmToolBar>();
		services.AddTransient<VmCandidatesBar>();
		services.AddTransient<VmInput>();
		services.AddTransient<VmClipboard>();
		services.AddTransient<VmRimeLog>();
		services.AddTransient<VmKey>();
		services.AddTransient<VmKeyBoard>();
		services.AddSingleton<ILogger>(_ => AppLog.Inst);
		var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = false, ValidateScopes = false });
		Di.SvcProvider = provider;

		var imeState = Di.GetRSvc<ImeState>();

		// 未處理按鍵轉發給 OS
		imeState.OsKeyProcessor = new AndroidOsKeyProcessor(() => CurrentInputConnection);

		// Rime commit 文字上屏
		imeState.OnCommit += (sender, text) => {
			var mainHandler = new Handler(Looper.MainLooper!);
			mainHandler.Post(() => {
				var ic = CurrentInputConnection;
				if (ic is not null) {
					ic.CommitText(text, 1);
					AppLog.Info($"[IME] CommitText: {text}");
				} else {
					AppLog.Warn("[IME] CommitText skipped: no InputConnection");
				}
			});
		};
	}

	public override void OnStartInputView(EditorInfo? info, bool restarting) {
		base.OnStartInputView(info, restarting);
		if (_shouldRecreateInputView) {
			SetInputView(OnCreateInputView());
		}
		AppLog.Info("[IME] OnStartInputView");
	}

	public override void OnWindowShown() {
		base.OnWindowShown();
		AppLog.Info($"[IME] OnWindowShown inputViewNull={_inputView is null} shouldRecreate={_shouldRecreateInputView}");
	}

	public override void OnWindowHidden() {
		base.OnWindowHidden();
		AppLog.Info("[IME] OnWindowHidden");
	}

	public void CommitText(str text) {
		var mainHandler = new Handler(Looper.MainLooper!);
		mainHandler.Post(() => {
			var ic = CurrentInputConnection;
			if (ic is not null) {
				ic.CommitText(text, 1);
			}
		});
	}

	public override void OnFinishInputView(bool finishingInput) {
		base.OnFinishInputView(finishingInput);
		_shouldRecreateInputView = true;
		AppLog.Info("[IME] OnFinishInputView");
	}

	public override void OnDestroy() {
		base.OnDestroy();
		AppLog.Info("[IME] OnDestroy");
	}
}

class AndroidStubOsKeyProcessor : IOsKeyProcessor {
	public event ErrHandler? OnErr;
	public Task<RespOnKeyEvent> OnKeyEventsAsy(IEnumerable<IKeyEvent> keyEvents)
		=> Task.FromResult(new RespOnKeyEvent());
}

class AndroidStubImeKeyProcessor : IImeKeyProcessor {
	public event ErrHandler? OnErr;
	public Task<RespOnKeyEvent> OnKeyEventsAsy(IEnumerable<IKeyEvent> keyEvents)
		=> Task.FromResult(new RespOnKeyEvent());
}
