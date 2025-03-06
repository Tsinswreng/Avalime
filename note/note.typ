=
[2025-03-04T22:51:31.281+08:00_W10-2]
```csharp

interface I_Key{
	///因keyCode可能因平臺而異 故用此作程序中key之唯一標識
	str name{get;set;}
}

class Key:I_Key{
	public str name{get;set;}="";
	public Key(){}
	public Key(str name){
		this.name = name
	}
}

static I_Key K(str name){
	var key = new Key(name);
	return key;
}

///按鍵池
class Keys{
	I_Key a = K(nameof(Keys.a));
	I_Key b = K(nameof(Keys.b));
	//...
	I_Key space = K(" ");
	I_Key tab = K("\t");
	I_Key return = K("\r");
	//...
	I_Key Ctrl_L = K( nameof(KeyNamesEnum.Ctrl_L));
	I_Key Ctrl_R = K( nameof(KeyNamesEnum.Ctrl_R));
	I_Key Win_L =  K(nameof(KeyNamesEnum.Win_L));
	I_Key Win_R =  K(nameof(KeyNamesEnum.Win_R));
}

interface I_KeyState{
	bool isKeyDown{get;set;}
}

interface I_KeyBoardState{
	/// isKeyDown(shift)
	bool isKeyDown(I_Key key);
	///當前所有已按下的鍵
	IEnumerable<I_Key> allDownKeys{get;set;}
}

interface I_KeyEvent{
	I_Key key{get;set;}
	I_KeyState keyState{get;set;}
	I_KeyBoardState? keyBoardState{get;set;}
}

interface I_onKeyEvent{
	onKeyEvent(I_KeyEvent keyEvent);
}

delegate object? errHandler(object sender, object? arg);

interface I_onErr{
	event errHandler errEvent;
}

interface I_KeyProcessor
	:I_onKeyEvent
	,I_onErr
{

}

///由輸入法引擎處理按鍵事件
interface I_ImeEngineKeyProcecssor
	:I_onKeyEvent
{

}

struct RimeKey{
	int keyCode;
	int mask;
}

///使用Rime輸入法引擎處理按鍵事件
class RimeKeyProcessor
	:I_ImeEngineKeyProcecssor
	,I_onErr
{
//Rime.process_key(RimeSessionId session_id, int keycode, int mask)
	onKeyEvent(I_KeyEvent keyEvent){
		try{
			RimeKey key = parseKeyEvent(keyEvent);
			var ans = rime.process_key(
				sessionId
				,key.keyCode
				,key.mask
			)
			if(ans == False){
				throw new SomeException("failed");
			}
		}catch(Ex e){
			//不隨便拋異常 以免引起輸入法UI端崩潰
			onErr.Invoke(this, e)
		}
	}
}



class WindowsKeyProcessor:I_onKeyEvent{
//注意擴展鍵
//keybd_event 已被取代、建議用SendInput https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-sendinput
//keybd_event((byte)Keys.F4, 0, 2, 0);        // 释放F4
}



interface I_KeyViewModel{
	Func click{get;set;}
	Func longClick{get;set;}
	Func swipeLeft{get;set;}
	Func swipeDown{get;set;}
	Func swipeUP{get;set;}
	Func swipeRight{get;set;}
}


class KeyBoardViewModelState{
	IDictionary<object, object?> config;
	object? getOption();
	object setOption();

	I_KeyProcessor osKeyProcessor;
	I_KeyProcessor imeKeyProcessor;

}


using KeyViewModelEvent = Func<xxx,xxx>;
class KeyViewModel:I_KeyViewModel{

	I_processKey keyProcessor;
	I_KeyBoardState keyBoardState//配㕥模擬組合鍵


	I_Key key;
	KeyViewModelEvent? click{get;set;}
	KeyViewModelEvent? longClick{get;set;}
	KeyViewModelEvent? swipeLeft{get;set;}
	KeyViewModelEvent? swipeDown{get;set;}
	KeyViewModelEvent? swipeUp{get;set;}
	KeyViewModelEvent? swipeRight{get;set;}

}
//send與commit
// 如上劃時直接繞過IME引擎而上屏、單擊時傳給IME引擎
// ascii模式

static IEnumerable<I_KeyEvent> ctrl_c(){
	var ctrl = Keys.Ctrl_L;
	var c = Keys.c;

	I_KeyEvent ctrlDown = new KeyEvent{
		key=ctrl
		,keyState=new KeyState{isKeyDown=true}
	};

	I_KeyEvent cDown = new KeyEvent{
		key=c
		,keyState=new KeyState{isKeyDown=true}
	};

	I_KeyEvent cUp = new KeyEvent{
		key=c
		,keyState=new KeyState{isKeyDown=false}
	};

	I_KeyEvent ctrlUp = new KeyEvent{
		key=ctrl
		,keyState=new KeyState{isKeyDown=up}
	};

	return [ctrlDown, cDown, cUp, ctrlUp];

}

var A = new KeyModel{
	click=KeyAction(Key=Keys.a)
	,swipeUp=new KeyAction(Key=Keys.A)
	,longClick=(o)=>{
		IEnumerable<I_KeyEvent> ctrl_C= ctrl_c();
		foreach(var e in ctrl_C){
			o.onKeyEvent(e);//
		}
	}
};

```

