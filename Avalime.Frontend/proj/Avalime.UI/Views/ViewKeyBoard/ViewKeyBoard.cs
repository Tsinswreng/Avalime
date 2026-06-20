using System.Collections.Generic;
using System.ComponentModel;
using Avalime.Core.Infra;
using Avalime.Core.Keys;
using Avalime.UI;
using Avalime.UI.Infra;
using ViewKeyControl = Avalime.UI.Views.ViewKey.ViewKey;
using VmKeyCtx = Avalime.UI.Views.ViewKey.VmKey;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Media;
using Microsoft.Extensions.DependencyInjection;
using static Avalime.Core.Keys.KeyChars;
using KS = Avalime.Core.Keys.KeyStates;

namespace Avalime.UI.Views.ViewKeyBoard;
using Ctx = VmKeyBoard;

public class ViewKeyBoard : AppViewBase<Ctx>
	, IDisposable
{
	readonly List<IDisposable> _disposables = [];
	readonly List<PropertyChangedEventHandler> _ctxHandlers = [];

	public ViewKeyBoard(){
		Ctx = Di.DiOrMk<Ctx>();
		Render();
	}

	GridStack Root = new(IsRow: true);
	Grid _mainKeys = default!;
	Grid _numKeys = default!;

	void Render(){
		this.SetContent(Root.Grid);
		var keyboardFont = UiCfg.Inst.KeyboardFontFamily;
		if(keyboardFont is not null){
			TextElement.SetFontFamily(Root.Grid, keyboardFont);
		}
		Root.SetRowDefs([new(1, GUT.Star)]);
		Root.A(MkKeysArea());
	}

	Grid MkKeysArea(){
		var panel = new Grid();
		_mainKeys = MkKeysGrid(isNum: false);
		_numKeys = MkKeysGrid(isNum: true);
		_numKeys.IsVisible = false;
		panel.Children.Add(_mainKeys);
		panel.Children.Add(_numKeys);

		PropertyChangedEventHandler onLayoutChanged = (s, e) => {
			if(e.PropertyName == nameof(Ctx.IsNumLayout)){
				_mainKeys.IsVisible = !Ctx.IsNumLayout;
				_numKeys.IsVisible = Ctx.IsNumLayout;
			}
		};
		_ctxHandlers.Add(onLayoutChanged);
		Ctx.PropertyChanged += onLayoutChanged;
		return panel;
	}

	Grid MkKeysGrid(bool isNum){
		var ans = new Grid{
			RowDefinitions = new("0.8*,*,*,*,*,0.8*"),
			Background = SolidColorBrush.Parse("#253238"),
		};
		i32 rowIdx = 0;
		if(isNum){
			AddRow(ans, MkNumRow1(), ref rowIdx);
			AddRow(ans, MkNumRow2(), ref rowIdx);
			AddRow(ans, MkNumRow3(), ref rowIdx);
			AddRow(ans, MkNumRow4(), ref rowIdx);
			AddRow(ans, MkNumRow5(), ref rowIdx);
			AddRow(ans, MkNumRow6(), ref rowIdx);
		}else{
			AddRow(ans, MkRow1(), ref rowIdx);
			AddRow(ans, MkRow2(), ref rowIdx);
			AddRow(ans, MkRow3(), ref rowIdx);
			AddRow(ans, MkRow4(), ref rowIdx);
			AddRow(ans, MkRow5(), ref rowIdx);
			AddRow(ans, MkRow6(), ref rowIdx);
		}
		return ans;
	}

	static void AddRow(Grid grid, Grid row, ref i32 idx){
		grid.Children.Add(row);
		Grid.SetRow(row, idx++);
	}

	readonly struct KeyCfg{
		public required IKeyChar Key{get;init;}
		public str? Label{get;init;}
		public str? Hint{get;init;}
		public str? HintBottom{get;init;}
		public IKeyChar? LongClick{get;init;}
		public Func<zero>? LongClickAction{get;init;}
		public IKeyChar? SwipeUp{get;init;}
		public IKeyChar? SwipeDown{get;init;}
		public Func<zero>? SwipeDownAction{get;init;}
		public IKeyChar? SwipeLeft{get;init;}
		public Func<zero>? SwipeLeftAction{get;init;}
		public IKeyChar? SwipeRight{get;init;}
		public Func<zero>? SwipeRightAction{get;init;}
		public Func<zero>? SwipeUpAction{get;init;}
		public bool IsRepeat{get;init;}
	}

	Grid MkRowCfg(params KeyCfg[] cfgs){
		var ans = new Grid();
		i32 col = 0;
		foreach(var _ in cfgs) ans.ColumnDefinitions.Add(new(1, GUT.Star));
		foreach(var cfg in cfgs){
			ans.Children.Add(KView(cfg));
			Grid.SetColumn(ans.Children[^1], col++);
		}
		return ans;
	}

	Grid MkRowOfControls(IList<Control> ctrls, IList<i32>? colWidths = null){
		var ans = new Grid();
		i32 col = 0;
		if(colWidths is not null){
			foreach(var w in colWidths) ans.ColumnDefinitions.Add(new(w, GUT.Star));
		}else{
			foreach(var _ in ctrls) ans.ColumnDefinitions.Add(new(1, GUT.Star));
		}
		foreach(var ctrl in ctrls){
			ans.Children.Add(ctrl);
			Grid.SetColumn(ans.Children[^1], col++);
		}
		return ans;
	}

	ViewKeyControl KView(KeyCfg cfg){
		var vm = Di.DiOrMk<VmKeyCtx>();
		_disposables.Add(vm);
		vm.Key_Click = cfg.Key;
		vm.Click = MkSendKey(cfg.Key);
		vm.FontSize = UiCfg.Inst.KeyFontSize;
		if(cfg.Label is not null) vm.Label = cfg.Label;
		if(cfg.Hint is not null) vm.Hint = cfg.Hint;
		if(cfg.HintBottom is not null) vm.BottomHint = cfg.HintBottom;
		vm.ImeState = Ctx.ImeState;
		if(cfg.LongClick is not null) vm.LongPress = MkSendKey(cfg.LongClick);
		if(cfg.LongClickAction is not null) vm.LongPress = cfg.LongClickAction;
		if(cfg.SwipeUp is not null) vm.SwipeUP = MkSendKey(cfg.SwipeUp);
		if(cfg.SwipeUpAction is not null) vm.SwipeUP = cfg.SwipeUpAction;
		if(cfg.SwipeDown is not null) vm.SwipeDown = MkSendKey(cfg.SwipeDown);
		if(cfg.SwipeDownAction is not null) vm.SwipeDown = cfg.SwipeDownAction;
		if(cfg.SwipeLeft is not null) vm.SwipeLeft = MkSendKey(cfg.SwipeLeft);
		if(cfg.SwipeLeftAction is not null) vm.SwipeLeft = cfg.SwipeLeftAction;
		if(cfg.SwipeRight is not null) vm.SwipeRight = MkSendKey(cfg.SwipeRight);
		if(cfg.SwipeRightAction is not null) vm.SwipeRight = cfg.SwipeRightAction;
		if(cfg.IsRepeat) vm.IsRepeat = true;
		if(cfg.Key == Dollar && cfg.Label == "$"){
			void SyncBg(){
				vm.Background = Ctx.IsShiftLocked ? UiCfg.Inst.MainColor : UiCfg.Inst.KeyBgColor;
			}
			SyncBg();
			_ctxHandlers.Add(OnKbPropertyChanged);
			Ctx.PropertyChanged += OnKbPropertyChanged;
			void OnKbPropertyChanged(object? sender, PropertyChangedEventArgs e){
				if(e.PropertyName == nameof(Ctx.IsShiftLocked)){
					SyncBg();
				}
			}
		}
		return new ViewKeyControl{Ctx = vm};
	}

	ViewKeyControl MkActionKey(str label, Action onClick, str? hint = null){
		var vm = Di.DiOrMk<VmKeyCtx>();
		_disposables.Add(vm);
		vm.Label = label;
		vm.FontSize = UiCfg.Inst.ActionKeyFontSize;
		if(hint is not null) vm.Hint = hint;
		vm.Click = () => { onClick(); return 0; };
		vm.ImeState = Ctx.ImeState;
		return new ViewKeyControl{Ctx = vm};
	}

	Func<zero> MkSendKey(IKeyChar key) => () => {
		Ctx.ImeState.InputSafely(MkKeyPressEvents(key));
		return 0;
	};

	IEnumerable<IKeyEvent> MkKeyPressEvents(IKeyChar key){
		if(Ctx.IsShiftLocked && key != Shift_L && key != Shift_R){
			var shiftedKey = ToShiftLockedKey(key);
			if(shiftedKey is not null){
				return [
					new KeyEvent{KeyChar = shiftedKey, KeyState = KS.Down},
					new KeyEvent{KeyChar = shiftedKey, KeyState = KS.Up}
				];
			}
			return [
				new KeyEvent{KeyChar = Shift_L, KeyState = KS.Down, KeyBoardState = KeyBoardState.Mk(Shift_L)},
				new KeyEvent{KeyChar = key, KeyState = KS.Down, KeyBoardState = KeyBoardState.Mk(Shift_L, key)},
				new KeyEvent{KeyChar = key, KeyState = KS.Up, KeyBoardState = KeyBoardState.Mk(Shift_L)},
				new KeyEvent{KeyChar = Shift_L, KeyState = KS.Up, KeyBoardState = KeyBoardState.Mk()},
			];
		}
		return [
			new KeyEvent{KeyChar = key, KeyState = KS.Down},
			new KeyEvent{KeyChar = key, KeyState = KS.Up}
		];
	}

	IKeyChar? ToShiftLockedKey(IKeyChar key){
		if(ShiftLockedKeyMap.TryGetValue(key, out var shifted)){
			return shifted;
		}
		return null;
	}

	static readonly IReadOnlyDictionary<IKeyChar, IKeyChar> ShiftLockedKeyMap = new Dictionary<IKeyChar, IKeyChar>{
		{a, A}, {b, B}, {c, C}, {d, D}, {e, E}, {f, F}, {g, G}, {h, H}, {i, I}, {j, J}, {k, K}, {l, L}, {m, M},
		{n, N}, {o, O}, {p, P}, {q, Q}, {r, R}, {s, S}, {t, T}, {u, U}, {v, V}, {w, W}, {x, X}, {y, Y}, {z, Z},
		{D1, Exclamation}, {D2, At}, {D3, HashTag}, {D4, Dollar}, {D5, Percent}, {D6, Caret}, {D7, Ampersand}, {D8, Asterisk}, {D9, Paren_L}, {D0, Paren_R},
		{Minus, Underscore}, {Equal, Plus},
		{SquareBracket_L, Braces_L}, {SquareBracket_R, Braces_R},
		{Semicolon, Colon}, {Apostrophe, Quote},
		{Comma, Less}, {Period, Greater}, {Slash, Question}, {BackSlash, BackSlash}, {Grave, Tilde},
	};

	Func<zero> MkToggleAsciiMode() => () => {
		Di.GetRSvc<RimeConnectionState>().ToggleAsciiMode();
		return 0;
	};

	Func<zero> MkToggleShiftLock() => () => {
		Ctx.IsShiftLocked = !Ctx.IsShiftLocked;
		return 0;
	};

	Func<zero> MkHideKeyboard() => () => {
		Di.GetRSvc<IKeyboardHost>().HideKeyboard();
		return 0;
	};

	Func<zero> MkSendCtrlKey(IKeyChar key) => () => {
		Ctx.ImeState.InputSafely(MkCtrlComboKeyEvents(key));
		return 0;
	};

	IEnumerable<IKeyEvent> MkCtrlComboKeyEvents(IKeyChar key){
		return [
			new KeyEvent{KeyChar = Ctrl_L, KeyState = KS.Down, KeyBoardState = KeyBoardState.Mk(Ctrl_L)},
			new KeyEvent{KeyChar = key, KeyState = KS.Down, KeyBoardState = KeyBoardState.Mk(Ctrl_L, key)},
			new KeyEvent{KeyChar = key, KeyState = KS.Up, KeyBoardState = KeyBoardState.Mk(Ctrl_L)},
			new KeyEvent{KeyChar = Ctrl_L, KeyState = KS.Up, KeyBoardState = KeyBoardState.Mk()},
		];
	}

	Func<zero> MkSendTextAndSpace(params IKeyChar[] keys) => () => {
		var keyEvents = new List<IKeyEvent>();
		foreach(var key in keys){
			keyEvents.AddRange(MkKeyPressEvents(key));
		}
		keyEvents.AddRange(MkKeyPressEvents(Space));
		Ctx.ImeState.InputSafely(keyEvents);
		return 0;
	};

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
		new(){Key=q, Label="Q", SwipeUp=Q},
		new(){Key=w, Label="W", SwipeUp=W},
		new(){Key=e, Label="E", SwipeUp=E},
		new(){Key=r, Label="R", SwipeUp=R},
		new(){Key=t, Label="T", SwipeUp=T},
		new(){Key=u, Label="U", SwipeUp=U, SwipeLeft=Paren_L, SwipeRight=Paren_R},
		new(){Key=i, Label="I", SwipeUp=I, SwipeLeft=SquareBracket_L, SwipeRight=SquareBracket_R},
		new(){Key=o, Label="O", SwipeUp=O, SwipeLeft=Braces_L, SwipeRight=Braces_R},
		new(){Key=p, Label="Π", SwipeUp=P, SwipeLeft=Less, SwipeRight=Greater},
		new(){Key=y, Label="Y", Hint="⇆", HintBottom="↷", SwipeUp=Y, SwipeLeftAction=MkToggleAsciiMode(), SwipeRightAction=MkSendCtrlKey(y), LongClickAction=MkSendCtrlKey(y)}
	);

	Grid MkRow3()=>MkRowCfg(
		new(){Key=a, Label="A", HintBottom="☑", SwipeUp=A, SwipeRightAction=MkSendCtrlKey(a), LongClickAction=MkSendCtrlKey(a)},
		new(){Key=s, Label="Σ", Hint="⇪", SwipeUp=S},
		new(){Key=d, Label="Δ", SwipeUp=D},
		new(){Key=f, Label="F", SwipeUp=F},
		new(){Key=g, Label="G", SwipeUp=G, SwipeLeft=Left, SwipeRight=Right, SwipeDown=Down},
		new(){Key=h, Label="H", SwipeUp=H},
		new(){Key=j, Label="J", SwipeUp=J},
		new(){Key=k, Label="K", SwipeUp=K},
		new(){Key=l, Label="Λ", SwipeUp=L},
		new(){Key=Semicolon, Label=";", Hint=":", SwipeUp=Colon, LongClick=Colon}
	);

	Grid MkRow4()=>MkRowCfg(
		new(){Key=z, Label="Z", HintBottom="↶", SwipeUp=Z, SwipeRightAction=MkSendCtrlKey(z), LongClickAction=MkSendCtrlKey(z)},
		new(){Key=x, Label="X", HintBottom="✁", SwipeUp=X, SwipeRightAction=MkSendCtrlKey(x), LongClickAction=MkSendCtrlKey(x)},
		new(){Key=c, Label="C", HintBottom="❐", SwipeUp=C, SwipeRightAction=MkSendCtrlKey(c), LongClickAction=MkSendCtrlKey(c)},
		new(){Key=v, Label="V", HintBottom="▣", SwipeUp=V, SwipeRightAction=MkSendCtrlKey(v), LongClickAction=MkSendCtrlKey(v)},
		new(){Key=b, Label="B", SwipeUp=B},
		new(){Key=n, Label="N", SwipeUp=N},
		new(){Key=m, Label="M", Hint="$m,", SwipeUp=M, SwipeDownAction=MkSendTextAndSpace(Dollar, m, Comma), SwipeLeftAction=MkSendTextAndSpace(Dollar, m, Comma, j), SwipeRightAction=MkSendTextAndSpace(Dollar, m, Comma, i)},
		new(){Key=Comma, Label=",", Hint="<", SwipeUp=Less, LongClick=Less, SwipeLeft=Less},
		new(){Key=Period, Label=".", Hint=">", SwipeUp=Greater, LongClick=Greater, SwipeRight=Greater},
		new(){Key=Apostrophe, Label="'", Hint="\"", SwipeUp=Quote}
	);

	Grid MkRow5(){
		var colWidths = new List<i32>{2,1,1,2,1,1,2};
		var ctrls = new List<Control>{
			KView(new(){Key=Enter, Label="↵"}),
			KView(new(){Key=Tab, Label="␉"}),
			KView(new(){Key=Left, Label="←", Hint="⇤"}),
			KView(new(){Key=Space, Label="", SwipeLeft=Left, SwipeRight=Right}),
			KView(new(){Key=Right, Label="→", Hint="⇥"}),
			KView(new(){Key=Dollar, Label="$", Hint="⇪", SwipeUpAction=MkToggleShiftLock()}),
			KView(new(){Key=Backspace, Label="⌫", LongClick=Backspace, SwipeUpAction=MkHideKeyboard(), IsRepeat=true}),
		};
		return MkRowOfControls(ctrls, colWidths);
	}

	Grid MkRow6(){
		var ctrls = new List<Control>{
			KView(new(){Key=Minus, Label="-", Hint="_", SwipeUp=Underscore}),
			KView(new(){Key=Equal, Label="=", Hint="+", SwipeUp=Plus}),
			KView(new(){Key=SquareBracket_L, Label="[", Hint="{", SwipeUp=Braces_L, LongClick=Braces_L}),
			KView(new(){Key=SquareBracket_R, Label="]", Hint="}", SwipeUp=Braces_R, LongClick=Braces_R}),
			KView(new(){Key=Up, Label="↑"}),
			KView(new(){Key=Down, Label="↓"}),
			KView(new(){Key=Slash, Label="/", Hint="?", SwipeUp=Question}),
			KView(new(){Key=BackSlash, Label="\\", Hint="|", SwipeUp=Pipe}),
			KView(new(){Key=Grave, Label="`", Hint="~", SwipeUp=Tilde}),
			MkActionKey("123", () => Ctx.IsNumLayout = true),
		};
		return MkRowOfControls(ctrls);
	}

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

	Grid MkNumRow2(){
		var colWidths = new List<i32>{1,1,2,2,2,1,1};
		var ctrls = new List<Control>{
			KView(new(){Key=q, Label="Q", SwipeUp=Q}),
			KView(new(){Key=w, Label="W", SwipeUp=W}),
			KView(new(){Key=D7}),
			KView(new(){Key=D8}),
			KView(new(){Key=D9, SwipeUp=I, LongClick=SquareBracket_L, SwipeLeft=SquareBracket_L, SwipeRight=SquareBracket_R}),
			KView(new(){Key=p, Label="Π", SwipeUp=P, SwipeLeft=Less, SwipeRight=Greater}),
			KView(new(){Key=y, Label="Y", Hint="⇆", HintBottom="↷", SwipeUp=Y, SwipeRightAction=MkSendCtrlKey(y), LongClickAction=MkSendCtrlKey(y)}),
		};
		return MkRowOfControls(ctrls, colWidths);
	}

	Grid MkNumRow3(){
		var colWidths = new List<i32>{1,1,2,2,2,1,1};
		var ctrls = new List<Control>{
			KView(new(){Key=a, Label="A", SwipeUp=A}),
			KView(new(){Key=s, Label="Σ", Hint="⇪", SwipeUp=S}),
			KView(new(){Key=D4, SwipeUp=D}),
			KView(new(){Key=D5, SwipeUp=G, SwipeLeft=Left, SwipeRight=Right, SwipeDown=Down}),
			KView(new(){Key=D6, SwipeUp=J}),
			KView(new(){Key=l, Label="Λ", SwipeUp=L}),
			KView(new(){Key=Semicolon, Label=";", Hint=":", SwipeUp=Colon, LongClick=Colon}),
		};
		return MkRowOfControls(ctrls, colWidths);
	}

	Grid MkNumRow4(){
		var colWidths = new List<i32>{1,1,2,2,2,1,1};
		var ctrls = new List<Control>{
			KView(new(){Key=z, Label="Z", SwipeUp=Z}),
			KView(new(){Key=x, Label="X", SwipeUp=X}),
			KView(new(){Key=D1, SwipeUp=C}),
			KView(new(){Key=D2, SwipeUp=B}),
			KView(new(){Key=D3, SwipeUp=M}),
			KView(new(){Key=Period, Label=".", Hint=">", SwipeUp=Greater, LongClick=Greater, SwipeRight=Greater}),
			KView(new(){Key=Apostrophe, Label="'", Hint="\"", SwipeUp=Quote}),
		};
		return MkRowOfControls(ctrls, colWidths);
	}

	Grid MkNumRow5(){
		var colWidths = new List<i32>{2,2,2,2,2};
		var ctrls = new List<Control>{
			KView(new(){Key=Enter, Label="↵"}),
			KView(new(){Key=D0}),
			KView(new(){Key=Space, Label="", SwipeLeft=Left, SwipeRight=Right}),
			KView(new(){Key=Period, Label=".", Hint=">", SwipeUp=Greater}),
			KView(new(){Key=Backspace, Label="⌫", LongClick=Backspace, SwipeUpAction=MkHideKeyboard(), IsRepeat=true}),
		};
		return MkRowOfControls(ctrls, colWidths);
	}

	Grid MkNumRow6(){
		var ctrls = new List<Control>{
			KView(new(){Key=Minus, Label="-", Hint="_", SwipeUp=Underscore}),
			KView(new(){Key=Equal, Label="=", Hint="+", SwipeUp=Plus}),
			KView(new(){Key=Slash, Label="/", Hint="?", SwipeUp=Question}),
			KView(new(){Key=BackSlash, Label="\\", Hint="|", SwipeUp=Pipe}),
			KView(new(){Key=Left, Label="←", Hint="⇤"}),
			KView(new(){Key=Right, Label="→", Hint="⇥"}),
			KView(new(){Key=Up, Label="↑"}),
			KView(new(){Key=Down, Label="↓"}),
			KView(new(){Key=Grave, Label="`", Hint="~", SwipeUp=Tilde}),
			MkActionKey("qwe", () => Ctx.IsNumLayout = false),
		};
		return MkRowOfControls(ctrls);
	}

	public void Dispose()
	{
		if(Ctx is not null){
			foreach(var handler in _ctxHandlers){
				Ctx.PropertyChanged -= handler;
			}
		}
		_ctxHandlers.Clear();
		foreach(var disposable in _disposables){
			disposable.Dispose();
		}
		_disposables.Clear();
	}
}
