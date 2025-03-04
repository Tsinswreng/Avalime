=
[2025-03-04T22:51:31.281+08:00_W10-2]
```csharp
// class KeyNamesEnum{
// 	str a = nameof(KeyNamesEnum.a);
// 	str b = "b";
// 	//...
// 	str A = "A";
// 	str B = "B";
// 	//...
// 	str space = " ";
// 	str tab = "\t";
// 	str return = "\r";
// 	//...
// 	str Ctrl_L = nameof(KeyNamesEnum.Ctrl_L);
// 	str Ctrl_R = nameof(KeyNamesEnum.Ctrl_R);
// 	str Win_L = nameof(KeyNamesEnum.Win_L);
// 	str Win_R = nameof(KeyNamesEnum.Win_R);
// }

interface I_Key{
	// int platformKeyCode{get;set;}
	// long uniKeyCode{get;set;}
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
	IEnumerable<I_Key> allDownKeys{get;set;}
}

interface I_KeyEvent{
	I_Key key{get;set;}
	I_KeyState keyState{get;set;}
	I_KeyBoardState keyBoardState{get;set;}
}

interface I_onKeyEvent{
	onKeyEvent(I_KeyEvent keyEvent);
}

delegate object? errHandler(object sender, object? arg);

interface I_onErr{
	//onErr();
	//IEnumerable<Func<object?, object?>> errHandlers{get;set;}
	event errHandler errEvent;
}

struct RimeKey{
	int keyCode;
	int mask;
}

class RimeKeyProcessor
	:I_onKeyEvent
	,I_onErr
{
//process_key(RimeSessionId session_id, int keycode, int mask)
	onKeyEvent(I_KeyEvent keyEvent){
		try{
			RimeKey key = parseKeyEvent(keyEvent);
			var ans = rime.process_key(
				sessionId
				,key.keyCode
				,key.mask
			)
			if(ans == False){
				throw new Exception();//TODO
			}
		}catch(Ex e){
			//不隨便拋異常 以免引起輸入法崩潰
			onErr.Invoke(this, e)
		}
	}
}

class WindowsKeyProcessor:I_onKeyEvent{
//注意擴展鍵
//keybd_event 已被取代、建議用SendInput https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-sendinput
//keybd_event((byte)Keys.F4, 0, 2, 0);        // 释放F4
}


class KeyViewModel{

	I_processKey keyProcessor;
	I_Key key;
	I_KeyBoardState keyBoardState//配㕥模擬組合鍵


	I_KeyEvent geneKeyEvent(){
		return new KeyEvent{
			key=this.key
			,keyBoardState=this.keyBoardState
			,keyState=new KeyState{keyDown=true}
		}
	}

	click(){

	}
	longClick(){

	}

	swipe(direction){

	}
}



```