namespace Avalime.Core.Keys;

public class KeyChar:IKeyChar{
	public str Name{get;set;}="";
	public KeyChar(){}
	public KeyChar(str name){
		this.Name = name;
	}
	public static IKeyChar k(str name){
		var key = new KeyChar(name);
		return key;
	}
}
