using Avalonia.Media;

namespace Avalime.UI;

public partial class UiCfg
{
	protected static UiCfg? _inst = null;
	public static UiCfg Inst => _inst ??= new UiCfg();

	public double KeyFontSize { get; set; } = 28.0;
	public double CandidateFontSize { get; set; } = 28.0;
	public IBrush MainColor { get; set; } = SolidColorBrush.Parse("#4DB6AC");
	public IBrush KeyBgColor { get; set; } = SolidColorBrush.Parse("#000000");
	public IBrush CandidateBgColor { get; set; } = SolidColorBrush.Parse("#1E2A32");
}
