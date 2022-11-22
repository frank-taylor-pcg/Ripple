using Ripple.Exceptions;
using Ripple.Statements;
using Ripple.Validators;

namespace Ripple.Keywords
{
	public class While : BlockStatement, IBlockParent
	{
		private readonly Func<bool>? Condition;

		public While(Func<bool> func, int lineNumber, string? expression) : base(lineNumber, expression)
		{
			Condition = func;
			Action = EvaluateLoopCondition;
		}

		public void ConstructBlock(List<Statement> statements, int startAddress)
		{
			LoopConstructor.ConstructLoop(statements, startAddress, typeof(While), typeof(EndWhile));
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
