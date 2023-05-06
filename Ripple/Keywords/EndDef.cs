using Ripple.Statements;

namespace Ripple.Keywords;

public class EndDef : Statement
{
	public EndDef(int lineNumber = -1, string? expression = null) : base(lineNumber, expression)
	{
	}
}