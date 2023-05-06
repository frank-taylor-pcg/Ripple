using Ripple.Statements;
using Ripple.Validators;
using System.Dynamic;
using System.Text;

namespace Ripple;

public class CodeBlock
{
	/// <summary>
	/// If this value is true, the code validation will throw an exception if a CodeBlock variable is used, but not created
	/// via the DeclareVariable command
	/// </summary>
	public bool UseExplicitVariableDeclarations { get; set; } = true;

	// I'm not happy about using a dynamic here, but I can't think of a cleaner way to accomplish this
	public readonly dynamic Mem = new ExpandoObject();
	public readonly List<Statement> Statements = new();

	public void SetVariable<T>(string variableName, T value)
	{
		((IDictionary<string, object>)Mem)[variableName] = value;
	}

	public void Validate()
	{
		try
		{
			BlockValidator.Validate(this);
		}
		catch (Exception ex)
		{
			Logger.Log(ex);
			throw;
		}
	}

	public void Add(Statement statement)
	{
		Statements.Add(statement);
		statement.Address = Statements.Count - 1;
	}

	public string GetCodeListing()
	{
		StringBuilder sb = new();
		foreach (Statement statement in Statements)
		{
			sb.AppendLine($"  {statement}");
		}
		return sb.ToString();
	}

	public string GetMemoryDump()
	{
		StringBuilder sb = new();
		foreach (KeyValuePair<string, object> entry in (IDictionary<string, object>)Mem)
		{
			sb.AppendLine($"  {entry.Key} : {entry.Value}");
		}
		return sb.ToString();
	}
	
	// public override string ToString()
	// {
	// 	StringBuilder sb = new();
	//
	// 	sb.AppendLine("\nCode Listing:");
	// 	sb.AppendLine(GetCodeListing());
	//
	// 	sb.AppendLine("Memory Dump:");
	// 	sb.AppendLine(GetMemoryDump());
	//
	// 	return sb.ToString();
	// }
}