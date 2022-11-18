using Ripple.Exceptions;
using Ripple.Statements;

namespace Ripple.Keywords
{
	public class For : BlockStatement
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
