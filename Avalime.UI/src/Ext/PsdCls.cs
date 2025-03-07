namespace Shr.Avalonia.util;

public class PsdCls{
	protected static PsdCls? _inst;
	public static PsdCls inst => _inst??= new PsdCls();
	public str pointerover=":"+nameof(pointerover);
	public str focus=":"+nameof(focus);
	public str pressed=":"+nameof(pressed);
}