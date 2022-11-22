using Ripple.Statements;
using Ripple.Validators;

namespace Ripple.Keywords
{
	public class ForEach : BlockStatement, IBlockParent
	{
		private int index = 0;
		private readonly IEnumerable<object> values;
		private readonly string variable;

		public ForEach(string variableName, IEnumerable<object> enumerable, int lineNumber = -1, string? expression = null)
			: base(lineNumber, expression)
		{
			variable = variableName;
			values = enumerable;
			Action = NextValue;
		}

		private void NextValue()
		{
			VM!.CodeBlock!.SetVariable(variable, values.ToList()[index]);
		}

		public void NextIndex()
		{
			if (++index >= values.Count())
			{
				Block!.Resolved = true;
			}
		}

		public void ConstructBlock(List<Statement> statements, int startAddress)
		{
			LoopConstructor.ConstructLoop(statements, startAddress, typeof(ForEach), typeof(EndForEach));
		}
	}
}
