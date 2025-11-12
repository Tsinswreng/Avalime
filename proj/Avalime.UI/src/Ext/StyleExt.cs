using Avalonia;
using Avalonia.Styling;

namespace Avalime.UI.Ext;

public static class StyleExt{
	public static zero set(this Style z, AvaloniaProperty property, object? value){
		var setter = new Setter(property, value);

		z.Setters.Add(setter);
		return 0;
	}
}