using System;
using System.Diagnostics;
using Avalime.Core.Keys;
using Avalime.ViewModels;
using KS = Avalime.Core.Keys.KeyStates;

namespace Avalime.ViewModels.key;
using Ctx = KeyVm;

public partial class KeyVm : ViewModelBase, IKeyViewModel
{
	public static Ctx Mk(){return new Ctx();}

	public KeyVm(){
		Label = Key_Click?.Name??"";

		Click = ()=>{
			var state = ImeState as ImeState;//TODO temp
			try{
				var sw = Stopwatch.StartNew();
				Debug.WriteLine($"[Perf] KeyVm.Click→Input start: {sw.ElapsedMilliseconds}ms");
				state?.Input([
					new KeyEvent{
						KeyChar = Key_Click,
						KeyState = KS.Down
					},
					new KeyEvent{
						KeyChar = Key_Click,
						KeyState = KS.Up
					}
				]);
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
}
