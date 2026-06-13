//鍵盤主視圖：候選欄 + 輸入區 + 六行按鍵
using System.Collections.Generic;
using Avalime.Core.Keys;
using Avalime.ViewModels.KeyBoard;
using Avalime.ViewModels.key;
using Avalonia.Controls;
using Avalime.UI.Views.input;
using Avalime.UI.Views.Key;
using Avalime.UI.Infra;

namespace Avalime.UI.Views.KeyBoard;
using Ctx = VmKeyBoard;

public class ViewKeyBoard : AppViewBase<Ctx>
{
	public ViewKeyBoard(){
		Ctx = Ctx.Mk();
		Render();
	}

	const i32 ColCnt = 11;
	GridStack Root = new(IsRow: true){Grid = {
		RowDefinitions = new("1*,2.5*,12*")
	}};

	void Render(){
		this.SetContent(Root.Grid);
		//輸入區
		Root.A(new ViewInput());
		//TopBar(候選欄)
		Root.A(new Avalime.UI.Views.topBar.ViewTopBar());
		//按鍵區
		Root.A(MkKeysGrid());
	}

	Grid MkKeysGrid(){
		var G = new Grid{RowDefinitions = new("*,*,*,*,*,*")};
		G.A(MkRow1());
		G.A(MkRow2());
		G.A(MkRow3());
		G.A(MkRow4());
		G.A(MkRow5());
		G.A(MkRow6());
		return G;
	}

	Grid MkRowOfKeys(IKeyChar[] Keys){
		var R = new Grid();
		foreach(var _ in Keys){
			R.ColumnDefinitions.Add(new(1, GUT.Star));
		}
		foreach(var k in Keys){
			R.A(KView(k));
		}
		return R;
	}

	Grid MkRowOfLabels(str[] Labels){
		var R = new Grid();
		foreach(var _ in Labels){
			R.ColumnDefinitions.Add(new(1, GUT.Star));
		}
		foreach(var l in Labels){
			R.A(MkKeyByLabel(l));
		}
		return R;
	}

	//從 IKeyChar 建一個 ViewKey
	ViewKey KView(IKeyChar Key){
		var Vm = KeyVm.Mk();
		Vm.Key_Click = Key;
		Vm.ImeState = Ctx!.ImeState;
		return new ViewKey{Ctx = Vm};
	}

	//從字串建一個 ViewKey
	ViewKey MkKeyByLabel(str Label){
		var Vm = KeyVm.Mk();
		Vm.Label = Label;
		Vm.ImeState = Ctx!.ImeState;
		return new ViewKey{Ctx = Vm};
	}

	#region 各行按鍵定義
	Grid MkRow1()=>MkRowOfKeys([
		KeyChars.D1, KeyChars.D2, KeyChars.D3, KeyChars.D4, KeyChars.D5,
		KeyChars.D6, KeyChars.D7, KeyChars.D8, KeyChars.D9, KeyChars.D0, KeyChars.Grave
	]);

	Grid MkRow2()=>MkRowOfKeys([
		KeyChars.q, KeyChars.w, KeyChars.e, KeyChars.r, KeyChars.t,
		KeyChars.y, KeyChars.u, KeyChars.i, KeyChars.o, KeyChars.p, KeyChars.SquareBracket_L
	]);

	Grid MkRow3()=>MkRowOfKeys([
		KeyChars.a, KeyChars.s, KeyChars.d, KeyChars.f, KeyChars.g,
		KeyChars.h, KeyChars.j, KeyChars.k, KeyChars.l, KeyChars.Semicolon, KeyChars.Apostrophe
	]);

	Grid MkRow4()=>MkRowOfKeys([
		KeyChars.z, KeyChars.x, KeyChars.c, KeyChars.v, KeyChars.b,
		KeyChars.n, KeyChars.m, KeyChars.Comma, KeyChars.Period, KeyChars.Backspace, KeyChars.Backspace
	]);

	//第5行混合 KeyChar 和字串
	Grid MkRow5(){
		var Keys = new List<Control>();
		Keys.Add(MkKeyByLabel("數"));
		Keys.Add(KView(KeyChars.Dollar));
		Keys.Add(KView(KeyChars.Up));
		Keys.Add(KView(KeyChars.Shift_L));
		for(var i=0;i<5;i++) Keys.Add(KView(KeyChars.Space));
		Keys.Add(KView(KeyChars.Enter));
		Keys.Add(KView(KeyChars.Enter));
		return MkRowOfControls(Keys);
	}

	Grid MkRow6(){
		return MkRowOfLabels(["⌨","←","↓","→","\\t","\\t","⇤","⇥","/","\\","⇪"]);
	}
	#endregion

	Grid MkRowOfControls(IList<Control> Ctrls){
		var R = new Grid();
		foreach(var _ in Ctrls){
			R.ColumnDefinitions.Add(new(1, GUT.Star));
		}
		foreach(var c in Ctrls){
			R.A(c);
		}
		return R;
	}
}
