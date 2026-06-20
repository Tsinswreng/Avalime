using System.Collections.ObjectModel;
using Avalonia.Threading;
using Avalime.Core.Infra.Log;
using Avalime.Rime;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Avalime.UI;

public class RimeLogBuffer : ObservableObject
	, IDisposable
{
	public ObservableCollection<string> Lines { get; } = [];

	readonly EventHandler<RimeLogEventArgs> _rimeLogHandler;
	const int MaxLines = 200;

	public RimeLogBuffer()
	{
		_rimeLogHandler = (_, e) => Append($"[{e.Level}] {e.Message}");
		RimeSetup.OnLog += _rimeLogHandler;
	}

	public void Append(string line)
	{
		Dispatcher.UIThread.Post(() => {
			Lines.Add(line);
			while(Lines.Count > MaxLines){
				Lines.RemoveAt(0);
			}
		});
	}

	public void AppendAndLog(Microsoft.Extensions.Logging.LogLevel level, string message)
	{
		AppLog.Inst.Log(level, 0, message, null, static (state, _) => state?.ToString() ?? "");
		Append($"[{level}] {message}");
	}

	public void Dispose()
	{
		RimeSetup.OnLog -= _rimeLogHandler;
	}
}
