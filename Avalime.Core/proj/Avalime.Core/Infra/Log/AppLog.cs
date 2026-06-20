namespace Avalime.Core.Infra.Log;

using Microsoft.Extensions.Logging;

public class AppLog:DelegatingLogger {
	public static AppLog Inst => field??=new AppLog();
}
