using Ripple.Statements;

namespace Ripple.Exceptions;

public class InvalidStatementException : Exception
{
	public InvalidStatementException(Statement statement)
		: base(CreateErrorMessage(statement))
	{ }

	private static string CreateErrorMessage(Statement statement)
	{
		string? expression = !string.IsNullOrEmpty(statement.Expression) ? $" {statement.Expression}" : null;

		return $"Statement is invalid [{statement.GetType().Name}]{expression} (Line {statement.Address})";
	}
}