using Ripple;
using System.Runtime.CompilerServices;

namespace RippleTest.ExampleLib;

internal static class Api
{
	internal static CodeBlock Write(
		this CodeBlock cb
		, Func<string> getValue
		, [CallerLineNumber] int lineNumber = -1
		, [CallerArgumentExpression("getValue")] string? expression = null)
	{
		cb.Add(new Write(getValue, lineNumber, expression));
		return cb;
	}
	internal static CodeBlock WriteLine(
		this CodeBlock cb
		, Func<string> getValue
		, [CallerLineNumber] int lineNumber = -1
		, [CallerArgumentExpression("getValue")] string? expression = null)
	{
		cb.Add(new WriteLine(getValue, lineNumber, expression));
		return cb;
	}
	internal static CodeBlock Banner(
		this CodeBlock cb
		, Func<string> getValue
		, int width = 40
		, [CallerLineNumber] int lineNumber = -1
		, [CallerArgumentExpression("getValue")] string? expression = null)
	{
		cb.Add(new Banner(getValue, width, lineNumber, expression));
		return cb;
	}

	internal static CodeBlock Iterate<T>(
		this CodeBlock cb
		, Action<T> action
		, IEnumerable<T> values
		, [CallerLineNumber] int lineNumber = -1
		, [CallerArgumentExpression("values")] string? valuesExpression = null
		, [CallerArgumentExpression("action")] string? actionExpression = null)
	{
		cb.Add(ExampleLib.Iterate.Create(action, values, lineNumber, $"{action} : {values}"));
		return cb;
	}

}