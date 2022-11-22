using Ripple.Keywords;
using Ripple.Statements;
using System.Runtime.CompilerServices;

namespace Ripple
{
	public static class API
	{
		public static CodeBlock DeclareVariable<T>(
			this CodeBlock cb
			, string name
			, T? value
			, [CallerLineNumber] int lineNumber = -1
			, [CallerArgumentExpression("name")] string? nameExpression = null
			, [CallerArgumentExpression("value")] string? valueExpression = null)
		{
			// Had to specify the namespace to make the compiler happy
			cb.Add(Keywords.DeclareVariable.Create<T>(cb, name, value, lineNumber, $"{nameExpression} = {valueExpression}"));
			return cb;
		}

		public static CodeBlock If(this CodeBlock cb, Func<bool> condition, [CallerLineNumber] int lineNumber = -1, [CallerArgumentExpression("condition")] string? expression = null)
		{
			cb.Add(new If(condition, lineNumber, expression));
			return cb;
		}
		public static CodeBlock ElseIf(this CodeBlock cb, Func<bool> condition, [CallerLineNumber] int lineNumber = -1, [CallerArgumentExpression("condition")] string? expression = null)
		{
			cb.Add(new ElseIf(condition, lineNumber, expression));
			return cb;
		}
		public static CodeBlock Else(this CodeBlock cb, [CallerLineNumber] int lineNumber = -1)
		{
			cb.Add(new Else(lineNumber));
			return cb;
		}
		public static CodeBlock EndIf(this CodeBlock cb, [CallerLineNumber] int lineNumber = -1)
		{
			cb.Add(new EndIf(lineNumber));
			return cb;
		}

		public static CodeBlock Switch(this CodeBlock cb, Func<object> valueLambda, [CallerLineNumber] int lineNumber = -1, [CallerArgumentExpression("valueLambda")] string? expression = null)
		{
			cb.Add(new Switch(valueLambda, lineNumber, expression));
			return cb;
		}
		public static CodeBlock Case(this CodeBlock cb, object value, [CallerLineNumber] int lineNumber = -1, [CallerArgumentExpression("value")] string? expression = null)
		{
			cb.Add(new Case(value, lineNumber, expression));
			return cb;
		}
		public static CodeBlock Break(this CodeBlock cb, [CallerLineNumber] int lineNumber = -1)
		{
			cb.Add(new Break(lineNumber));
			return cb;
		}
		public static CodeBlock Default(this CodeBlock cb, [CallerLineNumber] int lineNumber = -1)
		{
			cb.Add(new Default(lineNumber));
			return cb;
		}
		public static CodeBlock EndSwitch(this CodeBlock cb, [CallerLineNumber] int lineNumber = -1)
		{
			cb.Add(new EndSwitch(lineNumber));
			return cb;
		}

		public static CodeBlock While(this CodeBlock cb, Func<bool> condition, [CallerLineNumber] int lineNumber = -1, [CallerArgumentExpression("condition")] string? expression = null)
		{
			cb.Add(new While(condition, lineNumber, expression));
			return cb;
		}
		public static CodeBlock EndWhile(this CodeBlock cb, [CallerLineNumber] int lineNumber = -1)
		{
			cb.Add(new EndWhile(lineNumber));
			return cb;
		}

		public static CodeBlock Repeat(this CodeBlock cb, [CallerLineNumber] int lineNumber = -1)
		{
			cb.Add(new Repeat(lineNumber));
			return cb;
		}
		public static CodeBlock Until(this CodeBlock cb, Func<bool> condition, [CallerLineNumber] int lineNumber = -1, [CallerArgumentExpression("condition")] string? expression = null)
		{
			cb.Add(new Until(condition, lineNumber, expression));
			return cb;
		}

		public static CodeBlock CSAction(this CodeBlock cb, Action act, [CallerLineNumber] int lineNumber = -1, [CallerArgumentExpression("act")] string? expression = null)
		{
			cb.Add(new CSAction(act, lineNumber, expression));
			return cb;
		}

		//public static CodeBlock CSFunc<T>(Func<T> func, [CallerLineNumber] int lineNumber = -1, [CallerArgumentExpression("func")] string? expression = null)
		//{
		//	Action act = () =>
		//	{
		//		if (func is not null)
		//		{
		//			T? t = func!();
		//			VM!.Result = t!;
		//		}
		//	};
		//	cb.Add(new CSAction(act, lineNumber, expression));
		//	return cb;
		//}

		public static CodeBlock For(this CodeBlock cb,
			Func<bool> checkLambda,
			Action iteratorLambda,
			[CallerLineNumber] int lineNumber = -1,
			[CallerArgumentExpression("checkLambda")] string? checkString = null,
			[CallerArgumentExpression("iteratorLambda")] string? iteratorString = null
			)
		{
			string expression = $"{checkString} ; {iteratorString}";
			cb.Add(new For(checkLambda, iteratorLambda, lineNumber, expression));
			return cb;
		}
		public static CodeBlock EndFor(this CodeBlock cb, [CallerLineNumber] int lineNumber = -1)
		{
			cb.Add(new EndFor(lineNumber));
			return cb;
		}

		public static CodeBlock ForEach(
			this CodeBlock cb
			, string variable
			, IEnumerable<object> valueList
			, [CallerLineNumber] int lineNumber = -1
			, [CallerArgumentExpression("variable")] string? variableExpression = null
			, [CallerArgumentExpression("valueList")] string? valueListExpression = null)
		{
			cb.Add(new ForEach(variable, valueList, lineNumber, $"{variableExpression} in {valueListExpression}"));
			return cb;
		}

		public static CodeBlock EndForEach(this CodeBlock cb, [CallerLineNumber] int lineNumber = -1)
		{
			cb.Add(new EndForEach(lineNumber));
			return cb;
		}

		// TODO: Determine if functions are even necessary
		// TODO: There should be a more elegant way to accomplish function definitions and calls than this.
		public static CodeBlock Def(this CodeBlock cb, string functionName, List<Argument> argList, [CallerLineNumber] int lineNumber = -1)
		{
			string arguments = string.Join(", ", argList.Select(x => $"{x.Type.Name} {x.Name}").ToList());
			string expression = $"{functionName}({arguments})";
			cb.Add(new Def(functionName, argList, lineNumber, expression));
			return cb;
		}
		public static CodeBlock EndDef(this CodeBlock cb, [CallerLineNumber] int lineNumber = -1)
		{
			cb.Add(new EndDef(lineNumber));
			return cb;
		}
		public static CodeBlock Call(this CodeBlock cb, string functionName, List<Func<object>> parameterLambdas, [CallerLineNumber] int lineNumber = -1)
		{
			string arguments = string.Join(", ", parameterLambdas);
			string expression = $"{functionName}({arguments})";
			cb.Add(new Call(functionName, parameterLambdas, lineNumber, expression));
			return cb;
		}
	}
}
