using Avalime.Core.Infra;
using Avalonia.Media;
using Tsinswreng.CsCfg;

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
	public FontFamily? KeyboardFontFamily => GetKeyboardFontFamily();
	public IBrush MainColor { get; set; } = SolidColorBrush.Parse("#4DB6AC");
	public IBrush KeyBgColor { get; set; } = SolidColorBrush.Parse("#000000");
	public IBrush CandidateBgColor { get; set; } = SolidColorBrush.Parse("#1E2A32");

	FontFamily? GetKeyboardFontFamily(){
		var fontName = KeysCfg.Keyboard.Font.GetFrom(AppCfg.Inst);
		if(string.IsNullOrWhiteSpace(fontName)){
			return null;
		}
		try{
			var fontFamily = new FontFamily(fontName);
			var typeface = new Typeface(fontFamily);
			if(FontManager.Current.TryGetGlyphTypeface(typeface, out _)){
				return fontFamily;
			}
		}catch{
		}
		return null;
	}
}
