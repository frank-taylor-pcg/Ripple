using Ripple.Statements;

namespace Ripple.Keywords
{
	internal class DeclareVariable : Statement
	{
		public string Name { get; private set; }

		public DeclareVariable(string name, int lineNumber = -1, string? expression = null)
			: base(lineNumber, expression)
		{
			Name = name;
		}

		public static DeclareVariable Create<T>(CodeBlock cb, string name, T value, int lineNumber = -1, string? expression = null)
		{
			DeclareVariable result = new(name, lineNumber, expression);
			((IDictionary<string, object>)cb.Mem)[result.Name] = value;
			return result;
		}

	}
}
