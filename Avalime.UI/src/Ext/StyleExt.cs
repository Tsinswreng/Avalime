using Avalonia;
using Avalonia.Styling;

namespace Avalime.UI.Ext;

public static class StyleExt{
	public static zero set(this Style z, AvaloniaProperty property, object? value){
		z.Setters.Add(new Setter(property, value));
		return 0;
	}
}