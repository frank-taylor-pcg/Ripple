using Ripple.Statements;

namespace Ripple.Keywords
{
	public class Def : Statement
	{
		public object FunctionIdentifier { get; private set; }
		public List<Argument> Arguments { get; private set; }

		public Def(VirtualMachine? vm, object functionIdentifier, List<Argument> arguments, int lineNumber = -1, string? expression = null) : base(vm, lineNumber, expression)
		{
			FunctionIdentifier = functionIdentifier;
			Arguments = arguments;
		}
	}
}
