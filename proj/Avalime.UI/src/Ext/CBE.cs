//2025-03-09T21:11:06.192+08:00_W10-7
using System;
using System.Linq.Expressions;
using Avalonia.Data.Core;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Markup.Xaml.MarkupExtensions.CompiledBindings;
namespace Shr.Avalonia;

/// <summary>
//于c#中用編譯期綁定
// usage:
// using Ctx = MyDataContext;
//,new Binding(nameof(ctx.hasValue)) ->
//,new CBE(CBE.pth<Ctx, bool>(x=>x.hasValue))
//正則替換:
//new Binding(nameof(ctx\.(.*?)))
//new CBE(CBE.pth<Ctx, object?>(x=>x.$1))
//
/// </summary>
public class CBE : CompiledBindingExtension{
	public CBE(CompiledBindingPath path):base(path){}

	public static CompiledBindingPath pth<T>(
		Expression<Func<T, object?>> propertySelector
	){
		return pth<T, object?>(propertySelector);
	}



	public static CompiledBindingPath pth<T, Tar>(
		Expression<Func<T, Tar>> propertySelector)
	{
		var builder = new CompiledBindingPathBuilder();
		var body = propertySelector.Body;

		// 处理类型转换表达式（如值类型装箱）
		if (body is UnaryExpression { NodeType: ExpressionType.Convert } unaryExpr)
			body = unaryExpr.Operand;

		switch (body)
		{
			case MemberExpression memberExpr:  // 属性访问模式
				ProcessMemberExpression<T>(builder, memberExpr);
				break;
			case ParameterExpression paramExpr:  // 直接对象绑定模式
				ValidateObjectBinding(typeof(T), typeof(Tar));
				break;
			default:
				throw new ArgumentException("表达式必须为属性访问或对象绑定");
		}

		return builder.Build();
	}

	private static void ValidateObjectBinding(Type sourceType, Type targetType)
	{
		if (!targetType.IsAssignableFrom(sourceType))
			throw new InvalidOperationException($"类型不兼容：{sourceType}无法转换为{targetType}");
	}

	private static void ProcessMemberExpression<T>(CompiledBindingPathBuilder builder, MemberExpression expr)
	{
		var propName = expr.Member.Name;
		var propType = expr.Type;

		var clrProp = new ClrPropertyInfo(
			propName,
			obj => ((T)obj).GetType().GetProperty(propName)?.GetValue(obj),
			(obj, val) => ((T)obj).GetType().GetProperty(propName)?.SetValue(obj, val),
			propType
		);

		builder.Property(clrProp, PropertyInfoAccessorFactory.CreateInpcPropertyAccessor);
	}



	// public static CompiledBindingPath pth<T, Tar>(
	// 	Expression<Func<T, Tar>> propertySelector
	// ){
	// 	var builder = new CompiledBindingPathBuilder();
	// 	var body = propertySelector.Body;

	// 	// 处理类型转换表达式
	// 	if (body is UnaryExpression unaryExpr && unaryExpr.NodeType == ExpressionType.Convert){
	// 		body = unaryExpr.Operand;
	// 	}

	// 	if (!(body is MemberExpression memberExpr)){
	// 		throw new ArgumentException("表达式必须为属性访问");
	// 	}
	// 	//var memberExpr = (MemberExpression)propertySelector.Body;
	// 	var propName = memberExpr.Member.Name;

	// 	var clrProp = new ClrPropertyInfo(
	// 		propName,
	// 		obj => ((T)obj).GetType().GetProperty(propName).GetValue(obj),
	// 		(obj, val) => ((T)obj).GetType().GetProperty(propName).SetValue(obj, val),
	// 		typeof(Tar)
	// 	);
	// 	builder.Property(clrProp, PropertyInfoAccessorFactory.CreateInpcPropertyAccessor);
	// 	var path = builder.Build();
	// 	return path;
	// 	// var ans = new CompiledBindingExtension(path){

	// 	// };
	// 	// return ans;
	// }


}
