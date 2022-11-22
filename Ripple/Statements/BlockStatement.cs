namespace Ripple.Statements
{
	public abstract class BlockStatement : Statement
	{
		public BlockStatement(int lineNumber = -1, string? expression = null)
			: base(lineNumber, expression)
		{ }

		public Block? Block { get; set; }

		public override string ToString()
		{
			// Remove the extraneous bits from the name
			string name = GetType().Name.Split('`')[0];

			string? blockId = Block?.ID.ToString();

			if (blockId != null)
				blockId = blockId[^6..];

			return $"[VM/C# {Address,3}/{LineNumber,4}] : {name} {Expression}    (Block ID: {blockId})";
		}
	}
}
