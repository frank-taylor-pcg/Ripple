using Ripple.Statements;

namespace Ripple.Validators;

public interface IBlockParent
{
	void ConstructBlock(List<Statement> statements, int startAddress);
}