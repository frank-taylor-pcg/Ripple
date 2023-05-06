using Ripple.Statements;

namespace Ripple.Keywords;

/// <summary>
/// Marks the end of an If block and provides a jump target for when the last condition fails or the block is resolved.
/// </summary>
public class EndIf : BlockStatement
{
	public EndIf(int lineNumber) : base(lineNumber) { }

	public override bool IsValid() => Block!.IsValid;
}