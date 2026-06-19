using Avalime.Core.Infra;
using Avalonia;
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
		var fontPath = KeysCfg.Keyboard.Font.Path.GetFrom(AppCfg.Inst);
		if(string.IsNullOrWhiteSpace(fontPath) || !File.Exists(fontPath)){
			return null;
		}
		try{
			var familyName = KeysCfg.Keyboard.Font.Family.GetFrom(AppCfg.Inst);
			if(string.IsNullOrWhiteSpace(familyName)){
				familyName = GetFontFamilyNameFromFile(fontPath);
			}
			if(string.IsNullOrWhiteSpace(familyName)){
				return null;
			}
			var fontFamily = new FontFamily(new Uri(fontPath, UriKind.Absolute), familyName);
			var typeface = new Typeface(fontFamily);
			if(FontManager.Current.TryGetGlyphTypeface(typeface, out _)){
				return fontFamily;
			}
		}catch{
		}
		return null;
	}

	static string? GetFontFamilyNameFromFile(string fontPath){
		try{
			var resolverProp = typeof(AvaloniaLocator).GetProperty("Current", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
				?? typeof(AvaloniaLocator).GetProperty("CurrentMutable", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
			var resolver = resolverProp?.GetValue(null);
			if(resolver is null){
				return null;
			}
			var getService = resolver.GetType().GetMethod("GetService", [typeof(Type)]);
			var fontManagerType = Type.GetType("Avalonia.Platform.IFontManagerImpl, Avalonia.Base");
			if(getService is null || fontManagerType is null){
				return null;
			}
			var fontManagerImpl = getService.Invoke(resolver, [fontManagerType]);
			if(fontManagerImpl is null){
				return null;
			}
			var tryCreateGlyphTypeface = fontManagerType.GetMethod(
				"TryCreateGlyphTypeface",
				[
					typeof(Stream),
					typeof(FontSimulations),
					fontManagerType.Assembly.GetType("Avalonia.Media.IPlatformTypeface")!.MakeByRefType()
				]
			);
			if(tryCreateGlyphTypeface is null){
				return null;
			}
			using var stream = File.OpenRead(fontPath);
			object?[] args = [stream, FontSimulations.None, null];
			var ok = tryCreateGlyphTypeface.Invoke(fontManagerImpl, args) as bool?;
			if(ok == true && args[2] is not null){
				var familyNameProp = args[2]!.GetType().GetProperty("FamilyName");
				return familyNameProp?.GetValue(args[2]) as string;
			}
		}catch{
		}
		return null;
	}
}
