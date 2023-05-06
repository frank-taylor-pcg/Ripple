using Ripple.Statements;

namespace Ripple.Exceptions;

public class RippleException : Exception
{
	public RippleException(Statement statement, Exception? innerException = null)
		: base(CreateErrorMessage(statement), innerException)
	{ }

	private static string CreateErrorMessage(Statement statement)
	{
		string? expression = !string.IsNullOrEmpty(statement.Expression) ? $" {statement.Expression}" : null;

		return $"Error processing [{statement.GetType().Name}]{expression} (Line {statement.Address})";
	}
}