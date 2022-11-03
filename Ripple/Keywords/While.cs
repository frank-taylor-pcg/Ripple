using Ripple.Exceptions;
using Ripple.Statements;

namespace Ripple.Keywords
{
	public class While : BlockStatement
	{
		private readonly Func<bool>? Condition;

		public While(VirtualMachine vm, Func<bool> func, int lineNumber, string? expression) : base(vm, lineNumber, expression)
		{
			Condition = func;
			Action = EvaluateLoopCondition;
		}

		private void EvaluateLoopCondition()
		{
			Guards.ThrowIfNull(Block);
			Guards.ThrowIfNull(Condition);

			Block!.Resolved = false;

			// If the loop condition is true, just continue to the next step
			if (Condition!()) return;

			// Otherwise, mark the block as resolved and we'll move to the next address
			Block!.Resolved = true;
			int jumpAddress = Block.JumpTargets.Last();
			VM!.JumpTo(jumpAddress);
		}
	}
}
