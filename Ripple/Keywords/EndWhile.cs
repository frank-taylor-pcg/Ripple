using Ripple.Exceptions;
using Ripple.Statements;

namespace Ripple.Keywords
{
	public class EndWhile : BlockStatement
	{
		public EndWhile(int lineNumber) : base(lineNumber)
		{
			Action = JumpToWhile;
		}

		private void JumpToWhile()
		{
			Guards.ThrowIfNull(Block);

			// If the block is resolved just fall through
			if (Block!.Resolved) return;

			int jumpAddress = Block.Parent!.Address;
			VM!.JumpTo(jumpAddress);
		}
	}
}
