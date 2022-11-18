using Ripple.Statements;

namespace Ripple.Keywords
{
	public class Repeat : BlockStatement
	{
		public Repeat(int lineNumber) : base(lineNumber) { }

		public override bool IsValid() => Block!.IsValid;
	}
}
