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
	readonly PropertyChangedEventHandler _imePropertyChangedHandler;
	str _baseLabel = "";
	str _displayLabel = "";

	public VmKey(ISvcIme ImeState){
		this.ImeState = ImeState;
		// 先綁定 ImeState，再做 label 同步；split overlay 預熱時會在構造階段直接算顯示文本。
		// 若此時還沒賦值就讀 IsAsciiMode，會把整個 IME 啓動路徑崩掉。
		SyncDisplayLabel();
		_imePropertyChangedHandler = (_, e) => {
			if(e.PropertyName == nameof(ISvcIme.IsAsciiMode)){
				SyncDisplayLabel();
			}
		};
		this.ImeState.PropertyChanged += _imePropertyChangedHandler;

		Click = ()=>{
			var state = this.ImeState;
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

	public ISvcIme ImeState{get;set;}

	public IKeyChar Key_Click{
		get => field;
		set{
			_baseLabel = value.Name ?? "";
			SetProperty(ref field, value);
			SyncDisplayLabel();
		}
	}

	public str Label{
		get => _displayLabel;
		set{
			_baseLabel = value;
			SyncDisplayLabel();
		}
	}

	void SyncDisplayLabel()
	{
		var label = _baseLabel;
		if(ImeState is not null && ImeState.IsAsciiMode && label.Length == 1 && char.IsAsciiLetter(label[0])){
			label = char.ToLowerInvariant(label[0]).ToString();
		}
		if(_displayLabel == label){
			return;
		}
		_displayLabel = label;
		OnPropertyChanged(nameof(Label));
	}

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
		if(ImeState is not null){
			ImeState.PropertyChanged -= _imePropertyChangedHandler;
		}
	}
}
