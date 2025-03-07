namespace Avalime.Core.keys;

public class Key:I_Key{
	public str name{get;set;}="";
	public Key(){}
	public Key(str name){
		this.name = name;
	}
	public static I_Key k(str name){
		var key = new Key(name);
		return key;
	}
}
