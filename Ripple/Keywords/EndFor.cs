using Ripple.Statements;

namespace Ripple.Keywords
{
	public class EndFor : BlockStatement
	{
		public EndFor(VirtualMachine vm, int lineNumber) : base(vm, lineNumber)
		{
			Action = JumpToFor;
		}

		private void JumpToFor()
		{
			// If the block is resolved just fall through
			if (Block!.Resolved) return;

			if (Block.Parent is For @for)
			{
				@for.Iterator();
			}

			int jumpAddress = Block.Parent!.Address;
			VM!.JumpTo(jumpAddress);
		}
	}
}
