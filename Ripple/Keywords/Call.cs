using Ripple.Statements;

namespace Ripple.Keywords
{
	// Used to call a VM function
	public class Call : Statement
	{
		public object FunctionIdentifier { get; private set; }
		public List<Func<object>> Arguments { get; private set; }

		public Call(VirtualMachine? vm, object functionIdentifier, List<Func<object>> parameterLambdas,
			int lineNumber = -1, string? expression = null)

			: base(vm, lineNumber, expression)
		{
			FunctionIdentifier = functionIdentifier;
			Arguments = parameterLambdas;
		}
	}
}
