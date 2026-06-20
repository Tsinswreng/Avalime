using Microsoft.Extensions.Logging;

namespace Avalime.Rime;

public class RimeLogEventArgs : EventArgs
{
	public required LogLevel Level { get; init; }
	public required string Message { get; init; }
}
