using Ripple.Statements;

namespace Ripple.Keywords
{
	public class Switch : BlockStatement
	{
		public readonly Func<object> Value;

		public Switch(Func<object> getValue, int lineNumber, string? expression) : base(lineNumber, expression)
		{
			Value = getValue;
		}

		public override bool IsValid() => Block!.IsValid;
	}
}
