using Ripple.Exceptions;
using Ripple.Statements;
using Ripple.Validators;

namespace Ripple.Keywords
{
	/// <summary>
	/// The beginning condition of an If block
	/// </summary>
	public class If : BlockStatement, IBlockParent
	{
		public Func<bool> Condition { get; set; }

		public If(Func<bool> condition, int lineNumber, string? expression) : base(lineNumber, expression)
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

		public void ConstructBlock(List<Statement> statements, int startAddress)
		{
			// Do nothing if the statement isn't an IF statement
			if (statements[startAddress] is not If @if) return;

			int address = startAddress + 1;

			// Once we encounter an Else we can't add any more ElseIf or Else statements for this block
			bool endifExpected = false;

			@if.Block = new() { Parent = @if };

			while (address < statements.Count && !@if.Block.IsValid)
			{
				if (!endifExpected && statements[address] is ElseIf elseif)
				{
					@if.Block.AddJumpTarget(elseif);
				}

				if (!endifExpected && statements[address] is Else @else)
				{
					@if.Block.AddJumpTarget(@else);
					endifExpected = true;
				}

				if (statements[address] is EndIf endif)
				{
					@if.Block.AddJumpTarget(endif);
					@if.Block.IsValid = true;
				}

				address++;
			}
		}
	}
}
