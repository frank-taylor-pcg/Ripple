using Ripple.Exceptions;
using Ripple.Statements;

namespace Ripple.Keywords;

public class CsAction : Statement
{
	public CsAction(Action act, int lineNumber = -1, string? expression = null)
		: base(lineNumber, expression)
	{
		Guards.ThrowIfNull(act);

		Action = act;
	}
}