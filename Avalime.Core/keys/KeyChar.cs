namespace Avalime.Core.keys;

public class KeyChar:I_KeyChar{
	public str name{get;set;}="";
	public KeyChar(){}
	public KeyChar(str name){
		this.name = name;
	}
	public static I_KeyChar k(str name){
		var key = new KeyChar(name);
		return key;
	}
}
