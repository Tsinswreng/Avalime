//鍵盤主視圖：輸入區 + 候選欄 + 六行按鍵，佈局/提示/手勢兼容 TswG
using System.Collections.Generic;
using Avalime.Core.Keys;
using Avalime.ViewModels.KeyBoard;
using Avalime.ViewModels.key;
using Avalonia.Controls;
using Avalonia.Media;
using Avalime.UI.Views.input;
using Avalime.UI.Views.Key;
using Avalime.UI.Infra;
using static Avalime.Core.Keys.KeyChars;
using KS = Avalime.Core.Keys.KeyStates;

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
		var G = new Grid{
			RowDefinitions = new("0.8*,*,*,*,*,0.8*"),
			Background = SolidColorBrush.Parse("#253238"), //鍵盤區底色、匹配TswG keyboard_back_color
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

	#region KeyCfg
	/// 按鍵完整配置：鍵值、顯示標籤、提示文字、長按/滑動手勢
	readonly struct KeyCfg{
		public required IKeyChar Key{get;init;}
		public str? Label{get;init;}
		public str? Hint{get;init;}
		public IKeyChar? LongClick{get;init;}
		public IKeyChar? SwipeUp{get;init;}
		public IKeyChar? SwipeDown{get;init;}
		public IKeyChar? SwipeLeft{get;init;}
		public IKeyChar? SwipeRight{get;init;}
	}

	/// 以KeyCfg陣列建立等寬按鍵行
	Grid MkRowCfg(params KeyCfg[] Cfgs){
		var R = new Grid();
		i32 Col = 0;
		foreach(var _ in Cfgs)
			R.ColumnDefinitions.Add(new(1, GUT.Star));
		foreach(var c in Cfgs){
			R.Children.Add(KView(c));
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

	/// 從KeyCfg建立ViewKey、設定Vm的Label/Hint/手勢
	ViewKey KView(KeyCfg Cfg){
		var Vm = KeyVm.Mk();
		Vm.Key_Click = Cfg.Key;
		if(Cfg.Label is not null) Vm.Label = Cfg.Label;
		if(Cfg.Hint is not null) Vm.Hint = Cfg.Hint;
		Vm.ImeState = Ctx!.ImeState;
		if(Cfg.LongClick is not null) Vm.LongPress = MkSendKey(Cfg.LongClick);
		if(Cfg.SwipeUp is not null) Vm.SwipeUP = MkSendKey(Cfg.SwipeUp);
		if(Cfg.SwipeDown is not null) Vm.SwipeDown = MkSendKey(Cfg.SwipeDown);
		if(Cfg.SwipeLeft is not null) Vm.SwipeLeft = MkSendKey(Cfg.SwipeLeft);
		if(Cfg.SwipeRight is not null) Vm.SwipeRight = MkSendKey(Cfg.SwipeRight);
		return new ViewKey{Ctx = Vm};
	}

	/// 建立純標籤按鍵(無點擊動作)
	ViewKey MkKeyByLabel(str Label){
		var Vm = KeyVm.Mk();
		Vm.Label = Label;
		Vm.ImeState = Ctx!.ImeState;
		return new ViewKey{Ctx = Vm};
	}

	/// 建立發送按鍵事件的委託
	Func<zero> MkSendKey(IKeyChar K) => () => {
		var state = Ctx!.ImeState;
		state.Input([
			new KeyEvent{KeyChar = K, KeyState = KS.Down},
			new KeyEvent{KeyChar = K, KeyState = KS.Up}
		]);
		return 0;
	};
	#endregion

	#region 各行按鍵定義 — 與 TswG 鍵盤佈局一致，含 Hint 及手勢
	/// 第一行：數字 1-0，長按/上滑打出符號
	Grid MkRow1()=>MkRowCfg(
		new(){Key=D1, LongClick=Exclamation, SwipeUp=Exclamation},
		new(){Key=D2, LongClick=At, SwipeUp=At},
		new(){Key=D3, LongClick=HashTag, SwipeUp=HashTag},
		new(){Key=D4, LongClick=Dollar, SwipeUp=Dollar},
		new(){Key=D5, LongClick=Percent, SwipeUp=Percent},
		new(){Key=D6, LongClick=Caret, SwipeUp=Caret},
		new(){Key=D7, LongClick=Ampersand, SwipeUp=Ampersand},
		new(){Key=D8, LongClick=Asterisk, SwipeUp=Asterisk},
		new(){Key=D9, LongClick=Paren_L, SwipeUp=Paren_L},
		new(){Key=D0, LongClick=Paren_R, SwipeUp=Paren_R}
	);

	/// 第二行：Q W E R T U I O Π Y
	Grid MkRow2()=>MkRowCfg(
		new(){Key=q, Label="Q",                        SwipeUp=Q},
		new(){Key=w, Label="W",                        SwipeUp=W},
		new(){Key=e, Label="E",                        SwipeUp=E},
		new(){Key=r, Label="R",                        SwipeUp=R},
		new(){Key=t, Label="T",                        SwipeUp=T},
		new(){Key=u, Label="U",                        SwipeUp=U,   SwipeLeft=Paren_L, SwipeRight=Paren_R},
		new(){Key=i, Label="I",                        SwipeUp=I,   SwipeLeft=SquareBracket_L, SwipeRight=SquareBracket_R},
		new(){Key=o, Label="O",                        SwipeUp=O,   SwipeLeft=Braces_L, SwipeRight=Braces_R},
		new(){Key=p, Label="Π",                        SwipeUp=P,   SwipeLeft=Less, SwipeRight=Greater},
		new(){Key=y, Label="Y", Hint="⇆", SwipeUp=Y}
	);

	/// 第三行：A Σ Δ F G H J K Λ ;
	Grid MkRow3()=>MkRowCfg(
		new(){Key=a, Label="A",                         SwipeUp=A},
		new(){Key=s, Label="Σ", Hint="⇪", SwipeUp=S},
		new(){Key=d, Label="Δ",                         SwipeUp=D},
		new(){Key=f, Label="F",                         SwipeUp=F},
		new(){Key=g, Label="G",                         SwipeUp=G,   SwipeLeft=Left, SwipeRight=Right, SwipeDown=Down},
		new(){Key=h, Label="H",                         SwipeUp=H},
		new(){Key=j, Label="J",                         SwipeUp=J},
		new(){Key=k, Label="K",                         SwipeUp=K},
		new(){Key=l, Label="Λ",                         SwipeUp=L},
		new(){Key=Semicolon, Label=";", Hint=":",       SwipeUp=Colon, LongClick=Colon}
	);

	/// 第四行：Z X C V B N M , . '
	Grid MkRow4()=>MkRowCfg(
		new(){Key=z, Label="Z",                        SwipeUp=Z},
		new(){Key=x, Label="X",                        SwipeUp=X},
		new(){Key=c, Label="C",                        SwipeUp=C},
		new(){Key=v, Label="V",                        SwipeUp=V},
		new(){Key=b, Label="B",                        SwipeUp=B},
		new(){Key=n, Label="N",                        SwipeUp=N},
		new(){Key=m, Label="M", Hint="$m,",            SwipeUp=M},
		new(){Key=Comma, Label=",", Hint="<",          SwipeUp=Less,  LongClick=Less,  SwipeLeft=Less},
		new(){Key=Period, Label=".", Hint=">",         SwipeUp=Greater, LongClick=Greater, SwipeRight=Greater},
		new(){Key=Apostrophe, Label="'", Hint="\"",    SwipeUp=Quote}
	);

	/// 第五行：↵ ␉ ← (空格) → ⇪ ⌫ — 功能鍵行、部分鍵2倍寬
	Grid MkRow5(){
		var ColWidths = new List<i32>{2,1,1,2,1,1,2};
		var Ctrls = new List<Control>{
			//↵ Enter
			KView(new(){Key=Enter, Label="↵", Hint="↩"}),
			//␉ Tab
			KView(new(){Key=Tab, Label="␉"}),
			//← Left，長按/上滑 Home
			KView(new(){Key=Left, Label="←"}),
			//空格，左右滑動即 Left/Right
			KView(new(){Key=Space, Label="", SwipeLeft=Left, SwipeRight=Right}),
			//→ Right
			KView(new(){Key=Right, Label="→"}),
			//⇪ Shift，長按/上滑 $
			KView(new(){Key=Shift_L, Label="⇪", LongClick=Dollar, SwipeUp=Dollar}),
			//⌫ Backspace
			KView(new(){Key=Backspace, Label="⌫"}),
		};
		return MkRowOfControls(Ctrls, ColWidths);
	}

	/// 第六行：- = [ ] ↑ ↓ / \ ` 123 — 符號行、含提示及手勢
	Grid MkRow6(){
		var Ctrls = new List<Control>{
			KView(new(){Key=Minus, Label="-", Hint="_",              SwipeUp=Underscore}),
			KView(new(){Key=Equal, Label="=", Hint="+",             SwipeUp=Plus}),
			KView(new(){Key=SquareBracket_L, Label="[",             SwipeUp=Braces_L, LongClick=Braces_L}),
			KView(new(){Key=SquareBracket_R, Label="]",             SwipeUp=Braces_R, LongClick=Braces_R}),
			KView(new(){Key=Up, Label="↑"}),
			KView(new(){Key=Down, Label="↓"}),
			KView(new(){Key=Slash, Label="/", Hint="?",             SwipeUp=Question}),
			KView(new(){Key=BackSlash, Label="\\", Hint="|"}),
			KView(new(){Key=Grave, Label="`", Hint="~",             SwipeUp=Tilde}),
			MkKeyByLabel("123"), //切換數字鍵盤
		};
		return MkRowOfControls(Ctrls);
	}
	#endregion
}
