using Ripple.Exceptions;
using Ripple.Statements;

namespace Ripple.Keywords
{
	public class Break : BlockStatement
	{
		public Break(VirtualMachine vm, int lineNumber) : base(vm, lineNumber)
		{
			Action = JumpToEnd;
		}

		private void JumpToEnd()
		{
			Guards.ThrowIfNull(Block);

			Console.WriteLine("Break triggered");

			// Break statements mark the current block as resolved and jump to the End statement
			Block!.Resolved = true;
			int jumpAddress = Block.JumpTargets.Last();
			VM!.JumpTo(jumpAddress);
		}
	}
}
