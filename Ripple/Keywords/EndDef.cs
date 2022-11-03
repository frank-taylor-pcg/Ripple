using Ripple.Statements;

namespace Ripple.Keywords
{
	public class EndDef : Statement
	{
		public EndDef(VirtualMachine? vm, int lineNumber = -1, string? expression = null) : base(vm, lineNumber, expression)
		{
		}
	}
}
