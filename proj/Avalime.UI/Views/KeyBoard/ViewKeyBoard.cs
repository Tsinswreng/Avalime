//鍵盤主視圖：輸入區 + 候選欄 + 六行按鍵，佈局兼容 TswG
using System.Collections.Generic;
using Avalime.Core.Keys;
using Avalime.ViewModels.KeyBoard;
using Avalime.ViewModels.key;
using Avalonia.Controls;
using Avalonia.Media;
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

	GridStack Root = new(IsRow: true);

	void Render(){
		this.SetContent(Root.Grid);
		Root.SetRowDefs([
			new(1, GUT.Star),   //輸入區
			new(2.5, GUT.Star), //候選欄
			new(12, GUT.Star),  //按鍵區
		]);
		Root
		.A(new ViewInput())
		.A(new Avalime.UI.Views.topBar.ViewTopBar())
		.A(MkKeysGrid())
		;
	}

	/// 構建6行按鍵網格，Row1/Row6略矮(匹配TswG height_lower)
	Grid MkKeysGrid(){
		//Row1:0.8, Row2-5:1, Row6:0.8 — 上下行略矮
		var G = new Grid{
			RowDefinitions = new("0.8*,*,*,*,*,0.8*"),
			Background = SolidColorBrush.Parse("#253238"), //鍵盤區底色、匹配TswG鍵盤 keyboard_back_color
		};
		i32 RowIdx = 0;
		AddRow(G, MkRow1(), ref RowIdx);
		AddRow(G, MkRow2(), ref RowIdx);
		AddRow(G, MkRow3(), ref RowIdx);
		AddRow(G, MkRow4(), ref RowIdx);
		AddRow(G, MkRow5(), ref RowIdx);
		AddRow(G, MkRow6(), ref RowIdx);
		return G;
	}

	static void AddRow(Grid G, Grid Row, ref i32 Idx){
		G.Children.Add(Row);
		Grid.SetRow(Row, Idx++);
	}

	/// 以等寬列建立按鍵行
	Grid MkRowOfKeys(IKeyChar[] Keys){
		var R = new Grid();
		i32 Col = 0;
		foreach(var _ in Keys)
			R.ColumnDefinitions.Add(new(1, GUT.Star));
		foreach(var k in Keys){
			R.Children.Add(KView(k));
			Grid.SetColumn(R.Children[^1], Col++);
		}
		return R;
	}

	/// 以等寬列建立按鍵行、可指定自定義標籤(用于希臘字母等特殊顯示)
	Grid MkRowOfKeysLabeled(params (IKeyChar Key, str? Label)[] Pairs){
		var R = new Grid();
		i32 Col = 0;
		foreach(var _ in Pairs)
			R.ColumnDefinitions.Add(new(1, GUT.Star));
		foreach(var (Key, Label) in Pairs){
			R.Children.Add(KView(Key, Label));
			Grid.SetColumn(R.Children[^1], Col++);
		}
		return R;
	}

	/// 以元件列表建立按鍵行、可指定各列寬度(Star單位)
	Grid MkRowOfControls(IList<Control> Ctrls, IList<i32>? ColWidths = null){
		var R = new Grid();
		i32 Col = 0;
		if(ColWidths is not null){
			foreach(var w in ColWidths)
				R.ColumnDefinitions.Add(new(w, GUT.Star));
		}else{
			foreach(var _ in Ctrls)
				R.ColumnDefinitions.Add(new(1, GUT.Star));
		}
		foreach(var c in Ctrls){
			R.Children.Add(c);
			Grid.SetColumn(R.Children[^1], Col++);
		}
		return R;
	}

	/// 建立按鍵視圖、可選自定義顯示標籤
	/// <param name="Label">非null時覆蓋Key.Name作爲顯示文本</param>
	ViewKey KView(IKeyChar Key, str? Label = null){
		var Vm = KeyVm.Mk();
		Vm.Key_Click = Key;
		if(Label is not null)
			Vm.Label = Label;
		Vm.ImeState = Ctx!.ImeState;
		return new ViewKey{Ctx = Vm};
	}

	/// 建立純標籤按鍵(無點擊動作)
	ViewKey MkKeyByLabel(str Label){
		var Vm = KeyVm.Mk();
		Vm.Label = Label;
		Vm.ImeState = Ctx!.ImeState;
		return new ViewKey{Ctx = Vm};
	}

	#region 各行按鍵定義 — 與 TswG 鍵盤佈局一致
	/// 第一行：數字 1-0
	Grid MkRow1()=>MkRowOfKeys([
		KeyChars.D1, KeyChars.D2, KeyChars.D3, KeyChars.D4, KeyChars.D5,
		KeyChars.D6, KeyChars.D7, KeyChars.D8, KeyChars.D9, KeyChars.D0
	]);

	/// 第二行：Q W E R T U I O Π Y（TswG特有順序：U在I前、Y在末尾、Π爲P的標籤）
	Grid MkRow2()=>MkRowOfKeysLabeled(
		(KeyChars.q, "Q"), (KeyChars.w, "W"), (KeyChars.e, "E"),
		(KeyChars.r, "R"), (KeyChars.t, "T"), (KeyChars.u, "U"),
		(KeyChars.i, "I"), (KeyChars.o, "O"), (KeyChars.p, "Π"),
		(KeyChars.y, "Y")
	);

	/// 第三行：A Σ Δ F G H J K Λ ;（Σ/Δ/Λ爲希臘字母標籤）
	Grid MkRow3()=>MkRowOfKeysLabeled(
		(KeyChars.a, "A"), (KeyChars.s, "Σ"), (KeyChars.d, "Δ"),
		(KeyChars.f, "F"), (KeyChars.g, "G"), (KeyChars.h, "H"),
		(KeyChars.j, "J"), (KeyChars.k, "K"), (KeyChars.l, "Λ"),
		(KeyChars.Semicolon, null) //; 標籤與動作一致
	);

	/// 第四行：Z X C V B N M , . '
	Grid MkRow4()=>MkRowOfKeysLabeled(
		(KeyChars.z, "Z"), (KeyChars.x, "X"), (KeyChars.c, "C"),
		(KeyChars.v, "V"), (KeyChars.b, "B"), (KeyChars.n, "N"),
		(KeyChars.m, "M"), (KeyChars.Comma, null), //, 標籤與動作一致
		(KeyChars.Period, null), //. 標籤與動作一致
		(KeyChars.Apostrophe, null) //' 標籤與動作一致
	);

	/// 第五行：↵ ␉ ← (空格) → ⇪ ⌫ — 功能鍵行、部分鍵2倍寬
	Grid MkRow5(){
		//列寬比例：Enter(2):Tab(1):Left(1):Space(2):Right(1):Shift(1):Backspace(2) = 總10單位
		var ColWidths = new List<i32>{2,1,1,2,1,1,2};
		var Ctrls = new List<Control>{
			KView(KeyChars.Enter, "↵"),
			KView(KeyChars.Tab, "␉"),
			KView(KeyChars.Left, "←"),
			KView(KeyChars.Space, ""), //空格鍵無顯示標籤
			KView(KeyChars.Right, "→"),
			KView(KeyChars.Shift_L, "⇪"),
			KView(KeyChars.Backspace, "⌫"),
		};
		return MkRowOfControls(Ctrls, ColWidths);
	}

	/// 第六行：- = [ ] ↑ ↓ / \ ` 123 — 符號行、末尾爲數字鍵盤切換鍵
	Grid MkRow6(){
		var Ctrls = new List<Control>{
			KView(KeyChars.Minus, "-"),        // -
			KView(KeyChars.Equal, "="),         // =
			KView(KeyChars.SquareBracket_L, "["), // [
			KView(KeyChars.SquareBracket_R, "]"), // ]
			KView(KeyChars.Up, "↑"),            // ↑
			KView(KeyChars.Down, "↓"),          // ↓
			KView(KeyChars.Slash, "/"),          // /
			KView(KeyChars.BackSlash, "\\"),     // \
			KView(KeyChars.Grave, "`"),         // `
			MkKeyByLabel("123"),                 //切換數字鍵盤
		};
		return MkRowOfControls(Ctrls);
	}
	#endregion
}
