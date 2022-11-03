namespace Ripple.Statements
{
	public class BlockStatement : Statement
	{
		public BlockStatement(VirtualMachine? vm, int lineNumber = -1, string? expression = null)
			: base(vm, lineNumber, expression)
		{ }

		public Block? Block { get; set; }

		public override string ToString()
		{
			// Remove the extraneous bits from the name
			string name = GetType().Name.Split('`')[0];

			return $"[VM/C# {Address,3}/{LineNumber,4}] ({Block?.ID}): {name} {Expression}";
		}
	}
}
