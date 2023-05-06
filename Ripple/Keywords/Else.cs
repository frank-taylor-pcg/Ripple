using Ripple.Exceptions;
using Ripple.Statements;

namespace Ripple.Keywords;

/// <summary>
/// Secondary condition of an If block.  Multiple of these may exist in a single block.
/// </summary>
public class Else : BlockStatement
{
  public Else(int lineNumber) : base(lineNumber)
  {
    Action = DoAction;
  }

  private void DoAction()
  {
    Guards.ThrowIfNull(Block);

    // If the block is not resolved then this statement just falls through
    if (!Block!.Resolved) return;

    // Otherwise, jump to the final jump-target which should be an ENDIF
    int jumpAddress = Block!.JumpTargets.Last();
    Vm!.JumpTo(jumpAddress);
  }

  public override bool IsValid() => Block!.IsValid;

  public override string? GetValidationErrorMessage()
  {
    // If the Block is invalid then we're probably missing the EndIf, let the If statement handle the message
    return Block is null ? $"No matching {nameof(If)} for {this}" : null;
  }
}