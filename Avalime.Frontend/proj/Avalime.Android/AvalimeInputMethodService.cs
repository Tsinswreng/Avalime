using Android.App;
using Android.Content;
using Android.InputMethodServices;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Avalime.Core.Infra;
using Avalime.Core.Infra.Log;
using Avalime.Core.Keys;
using Avalime.Rime;
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
[IntentFilter(["android.view.InputMethod"])]
[MetaData("android.view.im", Resource = "@xml/ime_method")]
public class AvalimeInputMethodService : InputMethodService {
	public AvalimeInputMethodService() { }
	public AvalimeInputMethodService(nint javaReference, JniHandleOwnership transfer)
		: base(javaReference, transfer) { }

	LoggingAvaloniaView? _inputView{get;set;}
	bool _shouldRecreateInputView;

	/// 取得螢幕高度的一半，用於設定輸入法視窗高度。
	/// 若無法取得螢幕尺寸則回傳 <see cref="ViewGroup.LayoutParams.WrapContent"/>。
	int GetHalfScreenHeight() {
		var screenHeight = Resources?.DisplayMetrics?.HeightPixels ?? 0;
		return screenHeight > 0 ? screenHeight / 2 : ViewGroup.LayoutParams.WrapContent;
	}

	/// 評估當前是否應進入全螢幕輸入模式。
	/// Android 輸入法在橫屏等場景下預設會切換為全螢幕模式（佔滿整個螢幕、隱藏應用內容）。
	/// <returns>永遠回傳 <c>false</c>，強制禁用全螢幕模式，始終以嵌入式鍵盤視窗顯示。</returns>
	/// 此方法在輸入法視窗即將顯示時被系統調用。
	/// 回傳 <c>false</c> 可確保鍵盤不會遮擋整個螢幕，保持與普通軟鍵盤一致的行為。
	public override bool OnEvaluateFullscreenMode() {
		return false;
	}

	/// 配置輸入法視窗的佈局屬性。
	/// <param name="win">輸入法視窗。可能是候選窗 (<c>isCandidatesOnly=true</c>) 或完整輸入視窗。</param>
	/// <param name="isFullscreen">是否為全螢幕模式。<see cref="OnEvaluateFullscreenMode"/> 已強制回傳 <c>false</c>，故此處始終為 <c>false</c>。</param>
	/// <param name="isCandidatesOnly">是否僅顯示候選詞視窗（不包含鍵盤）。</param>
	/// 將視窗寬度設為 <see cref="ViewGroup.LayoutParams.MatchParent"/>（填滿螢幕寬度），
	/// 高度設為螢幕的一半（通過 <see cref="GetHalfScreenHeight"/>），確保鍵盤區域合理可視。
	public override void OnConfigureWindow(Window? win, bool isFullscreen, bool isCandidatesOnly) {
		base.OnConfigureWindow(win, false, isCandidatesOnly);
		win?.SetLayout(ViewGroup.LayoutParams.MatchParent, GetHalfScreenHeight());
	}

	/// 創建輸入法的主輸入視圖（鍵盤 + 候選欄等 UI）。
	/// 此方法是輸入法 UI 的入口，返回的 <see cref="global::Android.Views.View"/> 會嵌入到系統輸入法視窗中。
	/// <returns>包含 Avalonia 內容的 <see cref="LoggingAvaloniaView"/>，作為輸入法 UI 的根視圖。</returns>
	/// <para><b>調用時機：</b>系統在需要顯示輸入法鍵盤區域時調用。</para>
	/// <para><b>緩存邏輯：</b>
	///   若 <c>_inputView</c> 已存在且 <c>_shouldRecreateInputView</c> 為 <c>false</c>，則複用現有視圖，避免重複創建。
	///   若需要重建（例如橫豎屏切換後），會先 Dispose 舊的 Avalonia Content，再創建新實例。
	/// </para>
	/// <para><b>視圖內容：</b><see cref="MainView"/> 是 Avalime 的頂層 UI 組件，包含鍵盤、候選欄、工具欄等。</para>
	/// <para><b>佈局：</b>寬度填滿螢幕、高度為螢幕的一半。</para>
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

	/// 離開當前輸入視窗時請求隱藏鍵盤，並標記下次顯示時重建輸入視圖。
	/// 用於語言切換等場景 — 強制下次 <see cref="OnCreateInputView"/> 重新構建 UI，
	/// 以確保不同語言的鍵盤佈局被正確載入。
	public void HideKeyboardAndRecreateInputViewOnNextShow() {
		AppLog.Info("[IME] HideKeyboard requested");
		RequestHideSelf(0);
	}

	/// 輸入法服務創建時的回調。
	/// 在 Android 系統首次綁定輸入法服務時調用，是整個輸入法生命週期的起點。
	/// <para><b>執行順序：</b></para>
	/// <list type="number">
	/// <item>設定輸入法主題樣式（<c>MyTheme_Ime</c>）。</item>
	/// <item>構建 DI 容器，註冊所有服務：
	///   <list type="bullet">
	///     <item><see cref="IImeKeyProcessor"/> → <see cref="RimeKeyProcessor"/>（Rime 按鍵處理）</item>
	///     <item><see cref="IOsKeyProcessor"/> → <see cref="AndroidStubOsKeyProcessor"/>（stub，後續替換為 <see cref="AndroidOsKeyProcessor"/>）</item>
	///     <item><see cref="IKeyboardHost"/> → <see cref="AndroidKeyboardHost"/>（鍵盤宿主）</item>
	///     <item><see cref="IClipboardService"/> → <see cref="AndroidClipboardService"/>（剪貼簿）</item>
	///     <item><see cref="ISvcIme"/> → <c>Avalime.Rime.SvcIme</c>（Rime 引擎實例）</item>
	///     <item>各 ViewModel（VmIme、VmToolBar、VmKeyBoard 等）</item>
	///   </list>
	/// </item>
	/// <item>初始化 <see cref="ISvcIme"/>，設置狀態文字為 "正在連接 Rime"。</item>
	/// <item>配置 <see cref="IOsKeyProcessor"/> 為 <see cref="AndroidOsKeyProcessor"/>，將 Rime 未處理的按鍵轉發到 <see cref="InputMethodService.CurrentInputConnection"/>。</item>
	/// <item>訂閱 <see cref="ISvcIme.OnCommit"/>，在 main looper 上調用 <c>InputConnection.CommitText</c> 將 Rime commit 的文字上屏。</item>
	/// </list>
	public override void OnCreate() {
		SetTheme(Resource.Style.MyTheme_Ime);
		base.OnCreate();
		AppLog.Info("[IME] OnCreate");

		var services = new ServiceCollection();
		services.AddSingleton<RimeSetup>(_ => RimeSetup.Inst);
		services.AddSingleton<IImeKeyProcessor, RimeKeyProcessor>();
		services.AddSingleton<IOsKeyProcessor, AndroidStubOsKeyProcessor>();
		services.AddSingleton<IKeyboardHost>(_ => new AndroidKeyboardHost(() => this));
		services.AddSingleton<IClipboardService, AndroidClipboardService>();
		services.AddSingleton<ImeUiState>();
		services.AddSingleton<ISvcIme, SvcIme>();
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

		var imeState = Di.GetRSvc<ISvcIme>();
		imeState.StatusText = "正在連接 Rime";

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

	/// 輸入視圖即將顯示時的回調。
	/// 在編輯框獲得焦點、輸入法鍵盤即將彈出時由系統調用。
	/// <param name="info">當前編輯框的 <see cref="EditorInfo"/>，包含輸入類型、IME 選項等訊息。可為 <c>null</c>。</param>
	/// <param name="restarting">
	/// 是否為重啟場景。<c>true</c> 表示輸入法在同一編輯框中重新開始輸入（例如輸入類型切換），
	/// <c>false</c> 表示全新開始（例如焦點從一個編輯框移到另一個）。
	/// </param>
	/// 若 <c>_shouldRecreateInputView</c> 為 <c>true</c>（由 <see cref="OnFinishInputView"/> 或 <see cref="HideKeyboardAndRecreateInputViewOnNextShow"/> 設置），
	/// 則在此時重建輸入視圖。此延遲重建確保新視圖在正確的時機被系統接受。
	public override void OnStartInputView(EditorInfo? info, bool restarting) {
		base.OnStartInputView(info, restarting);
		if (_shouldRecreateInputView) {
			SetInputView(OnCreateInputView());
		}
		AppLog.Info("[IME] OnStartInputView");
	}

	/// 輸入法視窗已顯示時的回調。
	/// 在輸入法視窗對用戶可見後由系統調用，通常在動畫結束後觸發。
	/// 記錄當前 <c>_inputView</c> 是否為 <c>null</c> 及 <c>_shouldRecreateInputView</c> 標記的狀態，
	/// 用於調試視窗顯示異常（如空白鍵盤）問題。
	public override void OnWindowShown() {
		base.OnWindowShown();
		AppLog.Info($"[IME] OnWindowShown inputViewNull={_inputView is null} shouldRecreate={_shouldRecreateInputView}");
	}

	/// 輸入法視窗已隱藏時的回調。
	/// 在輸入法視窗完全不可見後由系統調用（例如用戶切換到其他 App、按下返回鍵隱藏鍵盤）。
	/// 視窗隱藏時不會立即銷毀輸入視圖，<c>_inputView</c> 會被保留以便下次快速顯示。
	public override void OnWindowHidden() {
		base.OnWindowHidden();
		AppLog.Info("[IME] OnWindowHidden");
	}

	/// 在 main looper 上向當前輸入連接提交文字（上屏）。
	/// 與 <see cref="ISvcIme.OnCommit"/> 訂閱不同，此方法是提供給外部（如 ViewModel）直接調用的提交入口。
	/// <param name="text">要上屏的文字。</param>
	/// 必須在主執行緒上操作 <see cref="InputMethodService.CurrentInputConnection"/>，
	/// 因此通過 <see cref="Handler"/> Post 到 <see cref="Looper.MainLooper"/>。
	public void CommitText(str text) {
		var mainHandler = new Handler(Looper.MainLooper!);
		mainHandler.Post(() => {
			var ic = CurrentInputConnection;
			if (ic is not null) {
				ic.CommitText(text, 1);
			}
		});
	}

	/// 輸入視圖結束時的回調。
	/// 在編輯框失去焦點或輸入法鍵盤即將隱藏時由系統調用。
	/// <param name="finishingInput">
	/// <c>true</c> 表示本次輸入會話完全結束（焦點離開編輯框、切換到其他 App）；
	/// <c>false</c> 表示僅是暫時隱藏或過渡狀態。
	/// </param>
	/// 設置 <c>_shouldRecreateInputView = true</c>，確保下次 <see cref="OnStartInputView"/> 時重建輸入視圖。
	/// 這樣做是為了應對 Android 系統在某些場景下會銷毀舊 View 的行為，
	/// 避免下次顯示時出現已失效的視圖引用導致空白或崩潰。
	public override void OnFinishInputView(bool finishingInput) {
		base.OnFinishInputView(finishingInput);
		_shouldRecreateInputView = true;
		AppLog.Info("[IME] OnFinishInputView");
	}

	/// 輸入法服務銷毀時的回調。
	/// 在 Android 系統終止輸入法服務時調用（如用戶卸載 App、系統回收記憶體、或進程被殺死）。
	/// 此為生命週期的最後一站。目前主要記錄日誌用於追蹤服務生命週期。
	/// 若需要在銷毀前釋放 Rime 引擎資源（如 <c>rime_finalize()</c>），應在此處添加清理邏輯。
	public override void OnDestroy() {
		base.OnDestroy();
		AppLog.Info("[IME] OnDestroy");
	}
}

class AndroidStubOsKeyProcessor : IOsKeyProcessor {
	public event ErrHandler? OnErr;
	public Task<IRespOnKeyEvent> OnKeyEvents(IEnumerable<IKeyEvent> keyEvents, CT Ct)
		=> Task.FromResult<IRespOnKeyEvent>(new RespOnKeyEvent());
}

class AndroidStubImeKeyProcessor : IImeKeyProcessor {
	public event ErrHandler? OnErr;
	public Task<IRespOnKeyEvent> OnKeyEvents(IEnumerable<IKeyEvent> keyEvents, CT Ct)
		=> Task.FromResult<IRespOnKeyEvent>(new RespOnKeyEvent());
}
