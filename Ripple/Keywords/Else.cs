using Ripple.Exceptions;
using Ripple.Statements;

namespace Ripple.Keywords
{
	/// <summary>
	/// Secondary condition of an If block.  Multiple of these may exist in a single block.
	/// </summary>
	public class Else : BlockStatement
	{
		public Else(VirtualMachine vm, int lineNumber) : base(vm, lineNumber)
		{
			Action = DoAction;
		}

		private void DoAction()
		{
			Guards.ThrowIfNull(Block);

			// If the block is already resolved jump to the final jump-target which should be an ENDIF
			if (Block!.Resolved)
			{
				int jumpAddress = Block!.JumpTargets.Last();
				VM!.JumpTo(jumpAddress);
			}
		}

		public override bool IsValid() => Block!.IsValid;

		public override string? GetValidationErrorMessage()
		{
			if (Block is null)
			{
				return $"No matching {nameof(If)} for {this}";
			}

			// If the Block is invalid then we're probably missing the EndIf, let the If statement handle the message
			return null;
		}
	}
}
