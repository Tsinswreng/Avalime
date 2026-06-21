using System;
using System.ComponentModel;
using Avalime.Core.Infra.Log;
using Avalime.Core.Keys;
using Avalime.UI;
using Avalime.ViewModels;
using Avalonia.Media;
using KS = Avalime.Core.Keys.KeyStates;

namespace Avalime.UI.Views.Key;
using Ctx = VmKey;

public partial class VmKey : ViewModelBase, IKeyViewModel
	, IDisposable
{
	string _normalLabel = "";
	readonly RimeConnectionState _rimeCon;
	readonly PropertyChangedEventHandler _rimeConnectionPropertyChangedHandler;

	public VmKey(RimeConnectionState RimeConnection){
		Label = Key_Click?.Name??"";

		_rimeCon = RimeConnection;
		_rimeConnectionPropertyChangedHandler = (_, e) => {
			if(e.PropertyName == nameof(_rimeCon.IsAsciiMode)){
				if(_rimeCon.IsAsciiMode){
					_normalLabel = Label;
					var name = Key_Click?.Name ?? "";
					if(name.Length == 1 && char.IsAsciiLetter(name[0])){
						Label = char.ToLowerInvariant(name[0]).ToString();
					}
				}else{
					Label = _normalLabel;
				}
			}
		};
		_rimeCon.PropertyChanged += _rimeConnectionPropertyChangedHandler;

		Click = ()=>{
			var state = ImeState as SvcState;
			try{
				var sw = System.Diagnostics.Stopwatch.StartNew();
				AppLog.Debug($"[Perf] VmKey.Click→Input start: {sw.ElapsedMilliseconds}ms");
				state?.InputSafely([
					new KeyEvent{
						KeyChar = Key_Click,
						KeyState = KS.Down
					},
					new KeyEvent{
						KeyChar = Key_Click,
						KeyState = KS.Up
					}
				], e => HandleErr(e));
				AppLog.Debug($"[Perf] VmKey.Click→Input done: {sw.ElapsedMilliseconds}ms");
			}
			catch(Exception e){
				HandleErr(e);
			}
			return 0;
		};
	}

	public Func<zero>? Click{get;set;}
	public Func<zero>? LongPress{get;set;}
	public Func<zero>? SwipeLeft{get;set;}
	public Func<zero>? SwipeDown{get;set;}
	public Func<zero>? SwipeUP{get;set;}
	public Func<zero>? SwipeRight{get;set;}

	public bool IsRepeat{get;set;}

	public SvcState ImeState{get;set;}

	public IKeyChar Key_Click{
		get => field;
		set{
			Label = value.Name??"";
			SetProperty(ref field, value);
		}
	}

	public str Label{
		get => field;
		set => SetProperty(ref field, value);
	} = "";

	public str Hint{
		get => field;
		set => SetProperty(ref field, value);
	} = "";

	public str BottomHint{
		get => field;
		set => SetProperty(ref field, value);
	} = "";

	public IBrush Background{
		get => field;
		set => SetProperty(ref field, value);
	} = UiCfg.Inst.KeyBgColor;

	public double FontSize{
		get => field;
		set => SetProperty(ref field, value);
	} = UiCfg.Inst.KeyFontSize;

	public void Dispose()
	{
		_rimeCon.PropertyChanged -= _rimeConnectionPropertyChangedHandler;
	}
}

