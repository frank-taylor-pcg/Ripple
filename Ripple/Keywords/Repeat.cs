using Ripple.Statements;

namespace Ripple.Keywords
{
	public class Repeat : BlockStatement
	{
		public Repeat(VirtualMachine? vm, int lineNumber) : base(vm, lineNumber) { }

		public override bool IsValid() => Block!.IsValid;
	}
}
