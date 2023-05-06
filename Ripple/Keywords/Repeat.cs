using Ripple.Statements;
using Ripple.Validators;

namespace Ripple.Keywords;

public class Repeat : BlockStatement, IBlockParent
{
	public Repeat(int lineNumber) : base(lineNumber) { }

	public void ConstructBlock(List<Statement> statements, int startAddress)
	{
		LoopConstructor.ConstructLoop(statements, startAddress, typeof(Repeat), typeof(Until));
	}

	public override bool IsValid() => Block!.IsValid;
}