using Ripple.Exceptions;
using Ripple.Statements;

namespace Ripple.Keywords
{
	public class Until : BlockStatement
	{
		private readonly Func<bool> Condition;

		public Until(VirtualMachine vm, Func<bool> func, int lineNumber, string? expression) : base(vm, lineNumber, expression)
		{
			Condition = func;
			Action = EvaluateLoopCondition;
		}

		private void EvaluateLoopCondition()
		{
			Guards.ThrowIfNull(Block);

			// If the loop condition is false, mark the block as resolved and fall through
			if (Condition())
			{
				Block!.Resolved = true;
				return;
			}

			int jumpAddress = Block!.Parent!.Address;
			VM!.JumpTo(jumpAddress);
		}
	}
}
