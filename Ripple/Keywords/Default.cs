using Ripple.Exceptions;
using Ripple.Statements;

namespace Ripple.Keywords;

public class Default : BlockStatement
{
	public Default(int lineNumber) : base(lineNumber)
	{
		Action = DoAction;
	}

	private void DoAction()
	{
		Guards.ThrowIfNull(Block);

		Block!.Resolved = true;
	}
}