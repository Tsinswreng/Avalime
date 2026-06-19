using Avalonia.Media;

namespace Avalime.UI;

public partial class UiCfg
{
	protected static UiCfg? _inst = null;
	public static UiCfg Inst => _inst ??= new UiCfg();

	public IBrush MainColor { get; set; } = SolidColorBrush.Parse("#4DB6AC");
	public IBrush KeyBgColor { get; set; } = SolidColorBrush.Parse("#000000");
}
