using Avalonia.Media;

namespace Avalime.UI;

public partial class UiCfg
{
	protected static UiCfg? _inst = null;
	public static UiCfg Inst => _inst ??= new UiCfg();

	public double BaseFontSize { get; set; } = 30.0;
	public double KeyFontSize => BaseFontSize * 0.9;
	public double CandidateFontSize => BaseFontSize * 0.9;
	public double ActionKeyFontSize => BaseFontSize * 0.72;
	public double HintFontSize => BaseFontSize * 0.46;
	public IBrush MainColor { get; set; } = SolidColorBrush.Parse("#4DB6AC");
	public IBrush KeyBgColor { get; set; } = SolidColorBrush.Parse("#000000");
	public IBrush CandidateBgColor { get; set; } = SolidColorBrush.Parse("#1E2A32");
}
