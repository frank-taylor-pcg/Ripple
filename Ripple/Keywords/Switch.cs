using Ripple.Statements;

namespace Ripple.Keywords
{
	public class Switch : BlockStatement
	{
		public readonly Func<object> Value;

		public Switch(VirtualMachine vm, Func<object> getValue, int lineNumber, string? expression) : base(vm, lineNumber, expression)
		{
			Value = getValue;
		}

		public override bool IsValid() => Block!.IsValid;
	}
}
