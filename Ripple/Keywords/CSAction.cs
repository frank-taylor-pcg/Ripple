using Ripple.Exceptions;
using Ripple.Statements;

namespace Ripple.Keywords
{
	public class CSAction : Statement
	{
		public CSAction(Action act, int lineNumber = -1, string? expression = null)
			: base(lineNumber, expression)
		{
			Guards.ThrowIfNull(act);

			Action = act;
		}
	}
}
