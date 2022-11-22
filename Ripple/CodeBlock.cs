using Ripple.Statements;
using Ripple.Validators;
using System.Dynamic;
using System.Text;

namespace Ripple
{
	public class CodeBlock
	{
		// I'm not happy about using a dynamic here, but I can't think of a cleaner way to accomplish this
		public dynamic Mem = new ExpandoObject();
		public List<Statement> Statements = new();

		public void SetVariable<T>(string variableName, T value)
		{
			((IDictionary<string, object>)Mem)[variableName] = value;
		}

		public void Validate()
		{
			try
			{
				BlockValidator.Validate(Statements);
			}
			catch (Exception ex)
			{
				Logger.Log(ex);
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

		public override string ToString()
		{
			StringBuilder sb = new();

			sb.AppendLine("\nCode Listing:");
			sb.AppendLine(GetCodeListing());

			sb.AppendLine("Memory Dump:");
			sb.AppendLine(GetMemoryDump());

			return sb.ToString();
		}
	}
}
