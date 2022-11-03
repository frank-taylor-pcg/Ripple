using Ripple.Exceptions;
using Ripple.Statements;

namespace Ripple.Keywords
{
	/// <summary>
	/// Secondary condition of an If block.  Multiple of these may exist in a single block.
	/// </summary>
	public class ElseIf : BlockStatement
	{
		public Func<bool> Condition { get; set; }

		public ElseIf(VirtualMachine vm, Func<bool> condition, int lineNumber, string? expression) : base(vm, lineNumber, expression)
		{
			Condition = condition;
			Action = DoAction;
		}

		private void DoAction()
		{
			Guards.ThrowIfNull(Block);
			Guards.ThrowIfNull(Condition);

			int jumpAddress = -1;

			// If the block is already resolved jump to the final jump-target which should be an ENDIF
			if (Block!.Resolved)
			{
				jumpAddress = Block!.JumpTargets.Last();
				VM!.JumpTo(jumpAddress);
				return;
			}

			// If the condition is true, then we just mark the block resolved and fall through to the next statement
			if (Condition())
			{
				Block!.Resolved = true;
				return;
			}

			// The IF statement is not added to the jump targets list because we can't jump to it, so the first element in the list should be either an ELSE-IF, ELSE, or ENDIF
			int index = Block.JumpTargets.IndexOf(Address);
			// TODO: This could throw an exception (it shouldn't, but it could)
			jumpAddress = Block.JumpTargets[index + 1];
			VM!.JumpTo(jumpAddress);
		}

		public override bool IsValid() => Block!.IsValid && Condition is not null;

		public override string? GetValidationErrorMessage()
		{
			if (Block is null)
			{
				return $"No matching {nameof(If)} for {this}";
			}

			if (Condition is null)
			{
				return $"Invalid condition for {this}";
			}

			// If the Block is invalid then we're probably missing the EndIf, let the If statement handle the message
			return null;
		}
	}
}
