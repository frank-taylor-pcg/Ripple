using Ripple.Exceptions;
using Ripple.Statements;

namespace Ripple.Keywords
{
	public class CSAction : Statement
	{
		public CSAction(VirtualMachine? vm, Action act, int lineNumber = -1, string? expression = null)
			: base(vm, lineNumber, expression)
		{
			Guards.ThrowIfNull(act);

			Action = act;
		}
	}
}
