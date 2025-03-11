using System.Text;

public partial class G{

	public static str logPath = "";
	static G(){
		logPath = getBaseDir() + "/debug.log";
	}


	/// <summary>
	/// get base dir of project  E:/_code/rime-tools
	/// the same as the dir of .gitignored file
	/// posix style path, not ends with "/"
	/// </summary>
	/// <returns></returns>
	public static str getBaseDir()
	{
		//dotnet run -> E:\_code\rime-tools\main\bin\Debug\net8.0\
		//dotnet test -> E:\_code\rime-tools\test\bin\Debug\net8.0\
		string domainDir = AppDomain.CurrentDomain.BaseDirectory;
		string baseDir = Path.GetFullPath(Path.Combine(domainDir, @"../../../../"));
		baseDir = baseDir.Replace("\\", "/");
		if (baseDir.EndsWith("/"))
		{
			baseDir = baseDir.Substring(0, baseDir.Length - 1);
		}
		return baseDir;
	}


	public static str debug(){
		File.AppendAllText(logPath, "\n");
		return "";
	}


	public static str debug(params unknown[] s){
		var sb = new StringBuilder();
		var ans = "";
		if (s == null)
		{
			ans = "\n";
		}
		else
		{
			for (var i = 0; i < s!.Length; i++)
			{
				sb.Append(s[i]?.ToString() ?? "");
				if (i < s!.Length - 1)
				{
					sb.Append(" ");
				}
			}
			ans = sb.ToString();
		}
#if true
		File.AppendAllText(logPath, ans + "\n");
#endif
		return ans;
	}
}
