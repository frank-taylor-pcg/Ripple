using Ripple.Exceptions;
using Ripple.Statements;

namespace Ripple.Keywords;

public class Break : BlockStatement
{
	public Break(int lineNumber) : base(lineNumber)
	{
		Action = JumpToEnd;
	}

	private void JumpToEnd()
	{
		Guards.ThrowIfNull(Block);

		// Break statements mark the current block as resolved and jump to the End statement
		Block!.Resolved = true;
		int jumpAddress = Block.JumpTargets.Last();
		Vm!.JumpTo(jumpAddress);
	}
}