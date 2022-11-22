using Ripple.Keywords;
using Ripple.Statements;

namespace Ripple.Keywords
{
	public class EndForEach : BlockStatement
	{
		public EndForEach(int lineNumber) : base(lineNumber)
		{
			Action = JumpToForEach;
		}

		private void JumpToForEach()
		{
			// Looking for the next index could resolve the block, so do it before the resolution check
			if (Block!.Parent is ForEach @foreach)
			{
				@foreach.NextIndex();
			}

			// If the block is resolved, just fall through
			if (Block.Resolved) return;

			int jumpAddress = Block.Parent!.Address;
			VM!.JumpTo(jumpAddress);
		}
	}
}
