namespace Avalime.Core.Infra;

using Tsinswreng.CsCfg;
using static Tsinswreng.CsCfg.CfgNode<obj?>;

public class KeysCfg{
	public static ICfgNode<str> RwCfgPath = Mk(null, [nameof(RwCfgPath)], "Avalime.Rw.jsonc");

	public static class Librime{
		public static ICfgNode _R = Mk(null, [nameof(Librime)]);
		public static ICfgNode<bool> AutoLinkDll = Mk(_R, [nameof(AutoLinkDll)], false);
		public static ICfgNode<str> DllPath = Mk(_R, [nameof(DllPath)], "");

		public static class RimeTraits{
			public static ICfgNode _R = Mk(null, [nameof(RimeTraits)]);
			public static ICfgNode<str> user_data_dir = Mk(_R, [nameof(user_data_dir)], "");
			public static ICfgNode<str> app_name = Mk(_R, [nameof(app_name)], "rime.avalime");
		}
	}
	public class Keyboard{
		public static ICfgNode _R = Mk(null, [nameof(Keyboard)]);
		/// <summary>
		/// 是否啓用分體鍵盤。用戶在工具欄手動切換後，將狀態持久化到可寫配置。
		/// </summary>
		public static ICfgNode<bool> IsSplitEnabled = Mk(_R, [nameof(IsSplitEnabled)], false);
		public class Font{
			public static ICfgNode _R = Mk(Keyboard._R, [nameof(Font)]);
			public static ICfgNode<str?> Path = Mk(_R, [nameof(Path)], (str?)null);
			public static ICfgNode<str?> Family = Mk(_R, [nameof(Family)], (str?)null);
			public static ICfgNode<f64> BaseFontSize = Mk(_R, [nameof(BaseFontSize)], 32.0);
		}

	}
}
