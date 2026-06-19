using System;
using System.Diagnostics;
using Avalime.Core.Keys;
using Avalime.UI;
using Avalime.ViewModels;
using Avalonia.Media;
using Microsoft.Extensions.DependencyInjection;
using KS = Avalime.Core.Keys.KeyStates;

namespace Avalime.ViewModels.key;
using Ctx = KeyVm;

public partial class KeyVm : ViewModelBase, IKeyViewModel
{
	public static Ctx Mk(){return new Ctx();}

	string _normalLabel = "";

	public KeyVm(){
		Label = Key_Click?.Name??"";

		var rimeCon = App.SvcP.GetRequiredService<RimeConnectionState>();
		rimeCon.PropertyChanged += (_, e) => {
			if(e.PropertyName == nameof(rimeCon.IsAsciiMode)){
				if(rimeCon.IsAsciiMode){
					_normalLabel = Label; // 保存非 ascii 標籤
					var name = Key_Click?.Name ?? "";
					// 只對單個拉丁字母鍵顯示小寫
					if(name.Length == 1 && char.IsAsciiLetter(name[0]))
						Label = char.ToLowerInvariant(name[0]).ToString();
				}else{
					Label = _normalLabel; // 恢復非 ascii 標籤
				}
			}
		};

		Click = ()=>{
			var state = ImeState as ImeState;//TODO temp
			try{
				var sw = Stopwatch.StartNew();
				Debug.WriteLine($"[Perf] KeyVm.Click→Input start: {sw.ElapsedMilliseconds}ms");
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
				Debug.WriteLine($"[Perf] KeyVm.Click→Input done: {sw.ElapsedMilliseconds}ms");
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

	/// <summary>長按後是否持續重複觸發 Click</summary>
	public bool IsRepeat{get;set;}

	public ImeState ImeState{get;set;}//TODO 改用接口

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

	/// 按鍵提示文字（顯示滑動/長按的結果）
	public str Hint{
		get => field;
		set => SetProperty(ref field, value);
	} = "";

	/// 按鍵底部提示文字（Ctrl 組合鍵等功能）
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
}
