using Ripple.Exceptions;
using Ripple.Statements;

namespace Ripple.Keywords
{
	public class Default : BlockStatement
	{
		public Default(VirtualMachine vm, int lineNumber) : base(vm, lineNumber)
		{
			Action = DoAction;
		}

		private void DoAction()
		{
			Guards.ThrowIfNull(Block);

			Block!.Resolved = true;
			return;
		}
	}
}
