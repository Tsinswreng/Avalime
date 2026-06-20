namespace Avalime.Core.Infra.Log;

using Microsoft.Extensions.Logging;
using Tsinswreng.CsCore;

[Doc($$"""
多態全局日志。適用于 [不想配依賴注入 又想據不同環境用不同日誌實現]旹。

如
public class AppLog:WrappedLog{...}

class MainActivity{
	OnCreate(){
		AppLog.Inst.Inner = new AndroidLogcat();
	}
}

class WindowsEntry{
	Main(){
		AppLog.Inst.Inner = new ConsoleLog();
	}
}

""")]
public interface IDelegatingLogger : ILogger {
	public ILogger Inner { get;set; }
}

public class DelegatingLogger : IDelegatingLogger {
	public ILogger Inner { get; set; }
	public DelegatingLogger(ILogger Inner) {
		this.Inner = Inner;
	}

	public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) {
		Inner.Log(logLevel, eventId, state, exception, formatter);
	}

	public bool IsEnabled(LogLevel logLevel) {
		return Inner.IsEnabled(logLevel);
	}

	public IDisposable? BeginScope<TState>(TState state) where TState : notnull {
		return Inner.BeginScope(state);
	}
}

