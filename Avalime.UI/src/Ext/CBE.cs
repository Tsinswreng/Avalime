using System;
using System.Linq.Expressions;
using System.Reactive.Linq;
using Avalime.UI.Ext;
using Avalime.ViewModels.key;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Core;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Markup.Xaml.MarkupExtensions.CompiledBindings;
using Avalonia.Media;
using Avalonia.Styling;
namespace Shr.Avalonia;
public class CBE : CompiledBindingExtension{
	public CBE(CompiledBindingPath path):base(path){}

	// public CBE(Func<CBE, CompiledBindingPath> f):base(f(this)){

	// }


	// public CompiledBindingPath cpth2<T, Tar>(
	// 	Expression<Func<T, string>> propertySelector
	// ){
	// 	return cpth<T, Tar>(propertySelector);
	// }

	public static CompiledBindingPath pth<T, Tar>(
		Expression<Func<T, string>> propertySelector
	){
		var builder = new CompiledBindingPathBuilder();
		var memberExpr = (MemberExpression)propertySelector.Body;
		var propName = memberExpr.Member.Name;

		var clrProp = new ClrPropertyInfo(
			propName,
			obj => ((T)obj).GetType().GetProperty(propName).GetValue(obj),
			(obj, val) => ((T)obj).GetType().GetProperty(propName).SetValue(obj, val),
			typeof(Tar)
		);
		builder.Property(clrProp, PropertyInfoAccessorFactory.CreateInpcPropertyAccessor);
		var path = builder.Build();
		return path;
		// var ans = new CompiledBindingExtension(path){

		// };
		// return ans;
	}


}
