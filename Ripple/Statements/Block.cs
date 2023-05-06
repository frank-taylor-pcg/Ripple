namespace Ripple.Statements;

public class Block
{
	/// <summary>
	/// Mostly for debugging, this provides a unique ID for the block
	/// </summary>
	public Guid Id { get; set; } = Guid.NewGuid();

	/// <summary>
	/// The statement that marks the entry point for the block
	/// </summary>
	public BlockStatement? Parent { get; init; }

	/// <summary>
	/// Once a conditional in a block has succeeded, we should skip subsequent conditional checks.  This flag allows that.
	/// </summary>
	public bool Resolved { get; set; }

	/// <summary>
	/// Because of their complexity, conditional blocks need to be carefully evaluated to make sure their structure is correct.
	/// </summary>
	public bool IsValid { get; set; }

	/// <summary>
	/// A collection of VM addresses that this can be jumped to within this block
	/// </summary>
	public List<int> JumpTargets { get; } = new();

	public void AddJumpTarget(BlockStatement statement)
	{
		statement.Block = this;
		JumpTargets.Add(statement.Address);
	}
}