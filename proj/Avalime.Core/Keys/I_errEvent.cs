namespace Avalime.Core.Keys;

public delegate object? errHandler(object sender, object? arg);

public interface I_onErr{
	public event errHandler? errEvent;
}