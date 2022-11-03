using Ripple.Statements;

namespace Ripple.Keywords
{
	/// <summary>
	/// Marks the end of a Switch block and provides a jump target for when the last condition fails or the block is resolved.
	/// </summary>
	public class EndSwitch : BlockStatement
	{
		public EndSwitch(VirtualMachine? vm, int lineNumber) : base(vm, lineNumber) { }

		public override bool IsValid() => Block!.IsValid;
	}
}
