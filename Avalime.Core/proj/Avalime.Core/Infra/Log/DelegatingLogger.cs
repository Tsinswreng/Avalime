namespace Avalime.Core.Infra.Log;

using Microsoft.Extensions.Logging;
using Tsinswreng.CsCore;

[Doc($$"""
多態全局日志。適用于 [不想配依賴注入 又想據不同環境用不同日誌實現]旹。

如
public class AppLog:DelegatingLogger{...}

class MainActivity{
	OnCreate(){
		AppLog.Inst.InnerLogger = new AndroidLogcat();
	}
}

class WindowsEntry{
	Main(){
		AppLog.Inst.InnerLogger = new ConsoleLog();
	}
}

""")]
public interface IDelegatingLogger : ILogger {
	public ILogger? InnerLogger { get;set; }
}

public class DelegatingLogger : IDelegatingLogger {
	public ILogger? InnerLogger { get; set; }
	public DelegatingLogger() { }
	public DelegatingLogger(ILogger InnerLogger) {
		this.InnerLogger = InnerLogger;
	}


	public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) {
		InnerLogger?.Log(logLevel, eventId, state, exception, formatter);
	}

	public bool IsEnabled(LogLevel logLevel) {
		return InnerLogger?.IsEnabled(logLevel) ?? false;
	}

	public IDisposable? BeginScope<TState>(TState state) where TState : notnull {
		return InnerLogger?.BeginScope(state);
	}
}

