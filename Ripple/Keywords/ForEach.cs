using Ripple.Statements;
using Ripple.Validators;

namespace Ripple.Keywords;

public class ForEach : BlockStatement, IBlockParent
{
	private int _index;
	private readonly IEnumerable<object> _values;
	private readonly string _variable;

	public ForEach(string variableName, IEnumerable<object> enumerable, int lineNumber = -1, string? expression = null)
		: base(lineNumber, expression)
	{
		_variable = variableName;
		_values = enumerable;
		Action = NextValue;
	}

	private void NextValue()
	{
		Vm!.CodeBlock!.SetVariable(_variable, _values.ToList()[_index]);
	}

	public void NextIndex()
	{
		if (++_index >= _values.Count())
		{
			Block!.Resolved = true;
		}
	}

	public void ConstructBlock(List<Statement> statements, int startAddress)
	{
		LoopConstructor.ConstructLoop(statements, startAddress, typeof(ForEach), typeof(EndForEach));
	}
}