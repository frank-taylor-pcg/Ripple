using Ripple.Exceptions;
using Ripple.Statements;

namespace Ripple.Keywords
{
	/// <summary>
	/// The beginning condition of an If block
	/// </summary>
	public class If : BlockStatement
	{
		public Func<bool> Condition { get; set; }

		public If(VirtualMachine vm, Func<bool> condition, int lineNumber, string? expression) : base(vm, lineNumber, expression)
		{
			Condition = condition;
			Action = DoAction;
		}

		private void DoAction()
		{
			Guards.ThrowIfNull(Block);
			Guards.ThrowIfNull(Condition);

			// If the condition is true, then we just mark the block resolved and fall through to the next statement
			if (Condition())
			{
				Block!.Resolved = true;
				return;
			}

			// The IF statement is not added to the jump targets list because we can't jump to it, so the first element in the list should be either an ELSE-IF, ELSE, or ENDIF
			int jumpAddress = Block!.JumpTargets.First();
			VM!.JumpTo(jumpAddress);
		}

		public override bool IsValid() => Block!.IsValid && Condition is not null;

		public override string? GetValidationErrorMessage()
		{
			if (Block is null)
			{
				return $"Fatal error when validating {this}";
			}
			else if (!Block.IsValid)
			{
				return $"No matching {nameof(EndIf)} found for {this}";
			}

			return $"Unknown error occurred when attempting to validate {this}";
		}
	}
}
