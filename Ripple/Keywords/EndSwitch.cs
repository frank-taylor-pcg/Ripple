using Ripple.Statements;

namespace Ripple.Keywords;

/// <summary>
/// Marks the end of a Switch block and provides a jump target for when the last condition fails or the block is resolved.
/// </summary>
public class EndSwitch : BlockStatement
{
	public EndSwitch(int lineNumber) : base(lineNumber) { }

	public override bool IsValid() => Block!.IsValid;
}