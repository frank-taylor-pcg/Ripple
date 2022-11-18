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

		internal void Add(Statement statement)
		{
			Statements.Add(statement);
			statement.Address = Statements.Count - 1;
		}

		public override string ToString()
		{
			StringBuilder sb = new();

			foreach (Statement statement in Statements)
			{
				sb.AppendLine(statement.ToString());
			}

			return sb.ToString();
		}
	}
}
