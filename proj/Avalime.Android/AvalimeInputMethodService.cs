using System.Diagnostics;
using Android.App;
using Android.Content;
using Android.InputMethodServices;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Avalonia.Android;
using Avalime.UI.Views;

namespace Avalime.Android;

[Service(
    Label = "Avalime",
    Permission = global::Android.Manifest.Permission.BindInputMethod,
    Exported = true)]
[IntentFilter(new[] { "android.view.InputMethod" })]
[MetaData("android.view.im", Resource = "@xml/ime_method")]
public class AvalimeInputMethodService : InputMethodService
{
    public AvalimeInputMethodService() { }
    public AvalimeInputMethodService(nint javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer) { }

    AvaloniaView? _inputView;

    int GetHalfScreenHeight()
    {
        var screenHeight = Resources?.DisplayMetrics?.HeightPixels ?? 0;
        return screenHeight > 0 ? screenHeight / 2 : ViewGroup.LayoutParams.WrapContent;
    }

    public override bool OnEvaluateFullscreenMode()
    {
        return false;
    }

    public override void OnConfigureWindow(Window? win, bool isFullscreen, bool isCandidatesOnly)
    {
        base.OnConfigureWindow(win, false, isCandidatesOnly);
        win?.SetLayout(ViewGroup.LayoutParams.MatchParent, GetHalfScreenHeight());
    }

    public override global::Android.Views.View OnCreateInputView()
    {
        Debug.WriteLine("[IME] OnCreateInputView");

        if (_inputView != null)
            return _inputView;

        _inputView = new AvaloniaView(this)
        {
            Content = new MainView()
        };
        _inputView.LayoutParameters = new ViewGroup.LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            GetHalfScreenHeight());

        return _inputView;
    }

    public override void OnCreate()
    {
        base.OnCreate();
        Debug.WriteLine("[IME] OnCreate");
    }

    public override void OnStartInputView(EditorInfo? info, bool restarting)
    {
        base.OnStartInputView(info, restarting);
        Debug.WriteLine("[IME] OnStartInputView");
    }

    public override void OnFinishInputView(bool finishingInput)
    {
        base.OnFinishInputView(finishingInput);
        Debug.WriteLine("[IME] OnFinishInputView");
    }
}
