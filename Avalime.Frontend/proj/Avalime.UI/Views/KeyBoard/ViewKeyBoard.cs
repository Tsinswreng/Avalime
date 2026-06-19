//鍵盤主視圖：輸入區 + 候選欄 + 六行按鍵，支援主鍵盤/數字鍵盤切換
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
using Avalime.UI;
using Microsoft.Extensions.DependencyInjection;
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
	Grid _mainKeys = default!;
	Grid _numKeys = default!;

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
		.A(MkKeysArea())
		;
	}

	/// 構建雙佈局切換容器
	Grid MkKeysArea(){
		var panel = new Grid();
		_mainKeys = MkKeysGrid(isNum: false);
		_numKeys = MkKeysGrid(isNum: true);
		_numKeys.IsVisible = false;
		panel.Children.Add(_mainKeys);
		panel.Children.Add(_numKeys);

		Ctx!.PropertyChanged += (s, e) => {
			if(e.PropertyName == nameof(Ctx.IsNumLayout)){
				_mainKeys.IsVisible = !Ctx.IsNumLayout;
				_numKeys.IsVisible = Ctx.IsNumLayout;
			}
		};
		return panel;
	}

	/// 構建6行按鍵網格
	Grid MkKeysGrid(bool isNum){
		var G = new Grid{
			RowDefinitions = new("0.8*,*,*,*,*,0.8*"),
			Background = SolidColorBrush.Parse("#253238"),
		};
		i32 RowIdx = 0;
		if(isNum){
			AddRow(G, MkNumRow1(), ref RowIdx);
			AddRow(G, MkNumRow2(), ref RowIdx);
			AddRow(G, MkNumRow3(), ref RowIdx);
			AddRow(G, MkNumRow4(), ref RowIdx);
			AddRow(G, MkNumRow5(), ref RowIdx);
			AddRow(G, MkNumRow6(), ref RowIdx);
		}else{
			AddRow(G, MkRow1(), ref RowIdx);
			AddRow(G, MkRow2(), ref RowIdx);
			AddRow(G, MkRow3(), ref RowIdx);
			AddRow(G, MkRow4(), ref RowIdx);
			AddRow(G, MkRow5(), ref RowIdx);
			AddRow(G, MkRow6(), ref RowIdx);
		}
		return G;
	}

	static void AddRow(Grid G, Grid Row, ref i32 Idx){
		G.Children.Add(Row);
		Grid.SetRow(Row, Idx++);
	}

	#region KeyCfg & helpers
	readonly struct KeyCfg{
		public required IKeyChar Key{get;init;}
		public str? Label{get;init;}
		public str? Hint{get;init;}
		public str? HintBottom{get;init;}
		public IKeyChar? LongClick{get;init;}
		public Func<zero>? LongClickAction{get;init;}
		public IKeyChar? SwipeUp{get;init;}
		public IKeyChar? SwipeDown{get;init;}
		public IKeyChar? SwipeLeft{get;init;}
		public Func<zero>? SwipeLeftAction{get;init;}
		public IKeyChar? SwipeRight{get;init;}
		public Func<zero>? SwipeRightAction{get;init;}
			public bool IsRepeat{get;init;}
	}

	Grid MkRowCfg(params KeyCfg[] Cfgs){
		var R = new Grid();
		i32 Col = 0;
		foreach(var _ in Cfgs) R.ColumnDefinitions.Add(new(1, GUT.Star));
		foreach(var c in Cfgs){
			R.Children.Add(KView(c));
			Grid.SetColumn(R.Children[^1], Col++);
		}
		return R;
	}

	Grid MkRowOfControls(IList<Control> Ctrls, IList<i32>? ColWidths = null){
		var R = new Grid();
		i32 Col = 0;
		if(ColWidths is not null){
			foreach(var w in ColWidths) R.ColumnDefinitions.Add(new(w, GUT.Star));
		}else{
			foreach(var _ in Ctrls) R.ColumnDefinitions.Add(new(1, GUT.Star));
		}
		foreach(var c in Ctrls){
			R.Children.Add(c);
			Grid.SetColumn(R.Children[^1], Col++);
		}
		return R;
	}

	ViewKey KView(KeyCfg Cfg){
		var Vm = KeyVm.Mk();
		Vm.Key_Click = Cfg.Key;
		if(Cfg.Label is not null) Vm.Label = Cfg.Label;
		if(Cfg.Hint is not null) Vm.Hint = Cfg.Hint;
		if(Cfg.HintBottom is not null) Vm.BottomHint = Cfg.HintBottom;
		Vm.ImeState = Ctx!.ImeState;
		if(Cfg.LongClick is not null) Vm.LongPress = MkSendKey(Cfg.LongClick);
		if(Cfg.LongClickAction is not null) Vm.LongPress = Cfg.LongClickAction;
		if(Cfg.SwipeUp is not null) Vm.SwipeUP = MkSendKey(Cfg.SwipeUp);
		if(Cfg.SwipeDown is not null) Vm.SwipeDown = MkSendKey(Cfg.SwipeDown);
		if(Cfg.SwipeLeft is not null) Vm.SwipeLeft = MkSendKey(Cfg.SwipeLeft);
		if(Cfg.SwipeLeftAction is not null) Vm.SwipeLeft = Cfg.SwipeLeftAction;
		if(Cfg.SwipeRight is not null) Vm.SwipeRight = MkSendKey(Cfg.SwipeRight);
		if(Cfg.SwipeRightAction is not null) Vm.SwipeRight = Cfg.SwipeRightAction;
		if(Cfg.IsRepeat) Vm.IsRepeat = true;
		return new ViewKey{Ctx = Vm};
	}

	/// 建立觸發自訂動作的按鍵（非發送按鍵事件）
	ViewKey MkActionKey(str Label, Action OnClick, str? Hint = null){
		var Vm = KeyVm.Mk();
		Vm.Label = Label;
		if(Hint is not null) Vm.Hint = Hint;
		Vm.Click = () => { OnClick(); return 0; };
		Vm.ImeState = Ctx!.ImeState;
		return new ViewKey{Ctx = Vm};
	}

	Func<zero> MkSendKey(IKeyChar K) => () => {
		var state = Ctx!.ImeState;
		state.InputSafely([
			new KeyEvent{KeyChar = K, KeyState = KS.Down},
			new KeyEvent{KeyChar = K, KeyState = KS.Up}
		]);
		return 0;
	};

	Func<zero> MkToggleAsciiMode() => () => {
		var rimeCon = App.SvcP.GetRequiredService<RimeConnectionState>();
		rimeCon.ToggleAsciiMode();
		return 0;
	};

	Func<zero> MkSendCtrlKey(IKeyChar K) => () => {
		var state = Ctx!.ImeState;
		state.InputSafely(MkCtrlComboKeyEvents(K));
		return 0;
	};

	IEnumerable<IKeyEvent> MkCtrlComboKeyEvents(IKeyChar K){
		return [
			new KeyEvent{KeyChar = Ctrl_L, KeyState = KS.Down, KeyBoardState = KeyBoardState.Mk(Ctrl_L)},
			new KeyEvent{KeyChar = K, KeyState = KS.Down, KeyBoardState = KeyBoardState.Mk(Ctrl_L, K)},
			new KeyEvent{KeyChar = K, KeyState = KS.Up, KeyBoardState = KeyBoardState.Mk(Ctrl_L)},
			new KeyEvent{KeyChar = Ctrl_L, KeyState = KS.Up, KeyBoardState = KeyBoardState.Mk()},
		];
	}
	#endregion

	#region 主鍵盤佈局（TswG default）
	Grid MkRow1()=>MkRowCfg(
		new(){Key=D1, Hint="!", LongClick=Exclamation, SwipeUp=Exclamation},
		new(){Key=D2, Hint="@", LongClick=At, SwipeUp=At},
		new(){Key=D3, Hint="#", LongClick=HashTag, SwipeUp=HashTag},
		new(){Key=D4, Hint="$", LongClick=Dollar, SwipeUp=Dollar},
		new(){Key=D5, Hint="%", LongClick=Percent, SwipeUp=Percent},
		new(){Key=D6, Hint="^", LongClick=Caret, SwipeUp=Caret},
		new(){Key=D7, Hint="&", LongClick=Ampersand, SwipeUp=Ampersand},
		new(){Key=D8, Hint="*", LongClick=Asterisk, SwipeUp=Asterisk},
		new(){Key=D9, Hint="(", LongClick=Paren_L, SwipeUp=Paren_L},
		new(){Key=D0, Hint=")", LongClick=Paren_R, SwipeUp=Paren_R}
	);

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
		new(){Key=y, Label="Y", Hint="⇆", HintBottom="↷", SwipeUp=Y, SwipeLeftAction=MkToggleAsciiMode(), SwipeRightAction=MkSendCtrlKey(y), LongClickAction=MkSendCtrlKey(y)}
	);

	Grid MkRow3()=>MkRowCfg(
		new(){Key=a, Label="A", HintBottom="☑",        SwipeUp=A, SwipeRightAction=MkSendCtrlKey(a), LongClickAction=MkSendCtrlKey(a)},
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

	Grid MkRow4()=>MkRowCfg(
		new(){Key=z, Label="Z", HintBottom="↶",       SwipeUp=Z, SwipeRightAction=MkSendCtrlKey(z), LongClickAction=MkSendCtrlKey(z)},
		new(){Key=x, Label="X", HintBottom="✁",       SwipeUp=X, SwipeRightAction=MkSendCtrlKey(x), LongClickAction=MkSendCtrlKey(x)},
		new(){Key=c, Label="C", HintBottom="❐",       SwipeUp=C, SwipeRightAction=MkSendCtrlKey(c), LongClickAction=MkSendCtrlKey(c)},
		new(){Key=v, Label="V", HintBottom="▣",       SwipeUp=V, SwipeRightAction=MkSendCtrlKey(v), LongClickAction=MkSendCtrlKey(v)},
		new(){Key=b, Label="B",                        SwipeUp=B},
		new(){Key=n, Label="N",                        SwipeUp=N},
		new(){Key=m, Label="M", Hint="$m,",            SwipeUp=M},
		new(){Key=Comma, Label=",", Hint="<",          SwipeUp=Less,  LongClick=Less,  SwipeLeft=Less},
		new(){Key=Period, Label=".", Hint=">",         SwipeUp=Greater, LongClick=Greater, SwipeRight=Greater},
		new(){Key=Apostrophe, Label="'", Hint="\"",    SwipeUp=Quote}
	);

	Grid MkRow5(){
		var ColWidths = new List<i32>{2,1,1,2,1,1,2};
		var Ctrls = new List<Control>{
			KView(new(){Key=Enter, Label="↵"}),
			KView(new(){Key=Tab, Label="␉"}),
			KView(new(){Key=Left, Label="←"}),
			KView(new(){Key=Space, Label="", SwipeLeft=Left, SwipeRight=Right}),
			KView(new(){Key=Right, Label="→"}),
			KView(new(){Key=Shift_L, Label="⇪", LongClick=Dollar, SwipeUp=Dollar}),
			KView(new(){Key=Backspace, Label="⌫", LongClick=Backspace, IsRepeat=true}),
		};
		return MkRowOfControls(Ctrls, ColWidths);
	}

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
			MkActionKey("123", () => Ctx!.IsNumLayout = true), //切換數字鍵盤
		};
		return MkRowOfControls(Ctrls);
	}
	#endregion

	#region 數字鍵盤佈局（TswGNum）
	// Row1: 同主鍵盤數字行
	Grid MkNumRow1()=>MkRowCfg(
		new(){Key=D1, Hint="!", LongClick=Exclamation, SwipeUp=Exclamation},
		new(){Key=D2, Hint="@", LongClick=At, SwipeUp=At},
		new(){Key=D3, Hint="#", LongClick=HashTag, SwipeUp=HashTag},
		new(){Key=D4, Hint="$", LongClick=Dollar, SwipeUp=Dollar},
		new(){Key=D5, Hint="%", LongClick=Percent, SwipeUp=Percent},
		new(){Key=D6, Hint="^", LongClick=Caret, SwipeUp=Caret},
		new(){Key=D7, Hint="&", LongClick=Ampersand, SwipeUp=Ampersand},
		new(){Key=D8, Hint="*", LongClick=Asterisk, SwipeUp=Asterisk},
		new(){Key=D9, Hint="(", LongClick=Paren_L, SwipeUp=Paren_L},
		new(){Key=D0, Hint=")", LongClick=Paren_R, SwipeUp=Paren_R}
	);

	// Row2: Q W [7] [8] [9] Π Y  (7/8/9 雙倍寬)
	Grid MkNumRow2(){
		var ColWidths = new List<i32>{1,1,2,2,2,1,1}; //總10單位
		var Ctrls = new List<Control>{
			KView(new(){Key=q, Label="Q", SwipeUp=Q}),
			KView(new(){Key=w, Label="W", SwipeUp=W}),
			KView(new(){Key=D7}), //7
			KView(new(){Key=D8}), //8
			KView(new(){Key=D9, SwipeUp=I, LongClick=SquareBracket_L, SwipeLeft=SquareBracket_L, SwipeRight=SquareBracket_R}), //9
			KView(new(){Key=p, Label="Π", SwipeUp=P, SwipeLeft=Less, SwipeRight=Greater}),
			KView(new(){Key=y, Label="Y", Hint="⇆", HintBottom="↷", SwipeUp=Y, SwipeRightAction=MkSendCtrlKey(y), LongClickAction=MkSendCtrlKey(y)}),
		};
		return MkRowOfControls(Ctrls, ColWidths);
	}

	// Row3: A Σ [4] [5] [6] Λ ;  (4/5/6 雙倍寬)
	Grid MkNumRow3(){
		var ColWidths = new List<i32>{1,1,2,2,2,1,1};
		var Ctrls = new List<Control>{
			KView(new(){Key=a, Label="A", SwipeUp=A}),
			KView(new(){Key=s, Label="Σ", Hint="⇪", SwipeUp=S}),
			KView(new(){Key=D4, SwipeUp=D}), //4
			KView(new(){Key=D5, SwipeUp=G, SwipeLeft=Left, SwipeRight=Right, SwipeDown=Down}), //5
			KView(new(){Key=D6, SwipeUp=J}), //6
			KView(new(){Key=l, Label="Λ", SwipeUp=L}),
			KView(new(){Key=Semicolon, Label=";", Hint=":", SwipeUp=Colon, LongClick=Colon}),
		};
		return MkRowOfControls(Ctrls, ColWidths);
	}

	// Row4: Z X [1] [2] [3] . '  (1/2/3 雙倍寬)
	Grid MkNumRow4(){
		var ColWidths = new List<i32>{1,1,2,2,2,1,1};
		var Ctrls = new List<Control>{
			KView(new(){Key=z, Label="Z", SwipeUp=Z}),
			KView(new(){Key=x, Label="X", SwipeUp=X}),
			KView(new(){Key=D1, SwipeUp=C}), //1 (via swipe)
			KView(new(){Key=D2, SwipeUp=B}), //2 (via swipe)
			KView(new(){Key=D3, SwipeUp=M}), //3 (via swipe)
			KView(new(){Key=Period, Label=".", Hint=">", SwipeUp=Greater, LongClick=Greater, SwipeRight=Greater}),
			KView(new(){Key=Apostrophe, Label="'", Hint="\"", SwipeUp=Quote}),
		};
		return MkRowOfControls(Ctrls, ColWidths);
	}

	// Row5: ↵ [0] [space] [.] ⌫  (全部雙倍寬)
	Grid MkNumRow5(){
		var ColWidths = new List<i32>{2,2,2,2,2}; //總10單位
		var Ctrls = new List<Control>{
			KView(new(){Key=Enter, Label="↵"}),
			KView(new(){Key=D0}), //0
			KView(new(){Key=Space, Label="", SwipeLeft=Left, SwipeRight=Right}),
			KView(new(){Key=Period, Label=".", Hint=">", SwipeUp=Greater}),
			KView(new(){Key=Backspace, Label="⌫", LongClick=Backspace, IsRepeat=true}),
		};
		return MkRowOfControls(Ctrls, ColWidths);
	}

	// Row6: - = / \ ← → ↑ ↓ ` 返回
	Grid MkNumRow6(){
		var Ctrls = new List<Control>{
			KView(new(){Key=Minus, Label="-", Hint="_",             SwipeUp=Underscore}),
			KView(new(){Key=Equal, Label="=", Hint="+",            SwipeUp=Plus}),
			KView(new(){Key=Slash, Label="/", Hint="?",            SwipeUp=Question}),
			KView(new(){Key=BackSlash, Label="\\", Hint="|"}),
			KView(new(){Key=Left, Label="←"}),
			KView(new(){Key=Right, Label="→"}),
			KView(new(){Key=Up, Label="↑"}),
			KView(new(){Key=Down, Label="↓"}),
			KView(new(){Key=Grave, Label="`", Hint="~",            SwipeUp=Tilde}),
			MkActionKey("返回", () => Ctx!.IsNumLayout = false), //返回主鍵盤
		};
		return MkRowOfControls(Ctrls);
	}
	#endregion
}
