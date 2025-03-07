namespace Avalime.Core.keys;

public class KeySymbol:I_KeySymbol{
	public str name{get;set;}="";
	public KeySymbol(){}
	public KeySymbol(str name){
		this.name = name;
	}
	public static I_KeySymbol k(str name){
		var key = new KeySymbol(name);
		return key;
	}
}
