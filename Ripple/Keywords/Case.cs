using Ripple.Exceptions;
using Ripple.Statements;

namespace Ripple.Keywords
{
	public class Case : BlockStatement
	{
		private readonly object Value;

		public Case(object value, int lineNumber, string? expression) : base(lineNumber, expression)
		{
			Value = value;
			Expression = expression;
			LineNumber = lineNumber;
			Action = DoAction;
		}

		private void DoAction()
		{
			Guards.ThrowIfNull(Block);

			// First we have to ensure that the parent statement is a switch statement
			if (Block!.Parent is Switch @switch)
			{
				// If the current value matches the parent's check value, then the block is resolved
				if (Value!.Equals(@switch.Value()))
				{
					Block.Resolved = true;
					return;
				}

				// If the block has been resolved, then we're falling through multiple cases and should
				// fall through this one as well
				if (Block.Resolved) return;

				int index = Block.JumpTargets.IndexOf(Address);
				// TODO: This could throw an exception (it shouldn't, but it could)
				int jumpAddress = Block.JumpTargets[index + 1];
				VM!.JumpTo(jumpAddress);
			}
		}
	}
}
