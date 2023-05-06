using Ripple.Exceptions;
using Ripple.Statements;

namespace Ripple.Keywords;

public class Case : BlockStatement
{
	private readonly object? _value;

	public Case(object value, int lineNumber, string? expression) : base(lineNumber, expression)
	{
		_value = value;
		Expression = expression;
		LineNumber = lineNumber;
		Action = DoAction;
	}

	private void DoAction()
	{
		Guards.ThrowIfNull(Block);

		if (Block!.Parent is not Switch @switch)
		{
			return;
		}

		// If the current value matches the parent's check value, then the block is resolved
		if (_value!.Equals(@switch.Value()))
		{
			Block.Resolved = true;
			return;
		}

		// If the block has been resolved, then we're falling through multiple cases and should
		// fall through this one as well
		if (Block.Resolved) return;

		int index = Block.JumpTargets.IndexOf(Address);
		// TODO: This could throw an exception (it shouldn't, but it could)
		int jumpAddress = Block.JumpTargets[index + 1];
		Vm!.JumpTo(jumpAddress);
	}
}