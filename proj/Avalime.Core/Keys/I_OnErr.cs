namespace Avalime.Core.Keys;

public delegate object? ErrHandler(object sender, object? arg);

public interface I_OnErr{
	public event ErrHandler? OnErr;
}
