using Ripple.Exceptions;
using Ripple.Statements;
using Ripple.Validators;

namespace Ripple.Keywords
{
	public class For : BlockStatement, IBlockParent
	{
		private readonly Func<bool> Check;
		public Action Iterator { get; private set; }

		public For(Func<bool> checkLambda, Action iteratorLambda, int lineNumber = -1, string? expression = null)
			: base(lineNumber, expression)
		{
			Check = checkLambda;
			Iterator = iteratorLambda;
			Action = IterateRange;
		}

		public void ConstructBlock(List<Statement> statements, int startAddress)
		{
			LoopConstructor.ConstructLoop(statements, startAddress, typeof(For), typeof(EndFor));
		}

		private void IterateRange()
		{
			Guards.ThrowIfNull(Block);
			Guards.ThrowIfNull(Action);
			Guards.ThrowIfNull(Iterator);
			Guards.ThrowIfNull(Check);

			if (!Check())
			{
				Block!.Resolved = true;
				int jumpAddress = Block.JumpTargets.Last();
				VM!.JumpTo(jumpAddress);
			}
		}
	}
}
