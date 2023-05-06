using Ripple.Statements;

namespace RippleTest.ExampleLib;

/// <summary>
/// An example of an added keyword that writes the supplied text value to the console. The use of a Func
/// to get the string value allows it to be evaluated when this command is called, not when it is created.
/// </summary>
internal class Write : Statement
{
	public Write(Func<string> getValue, int lineNumber = -1, string? expression = null)
		: base(lineNumber, expression)
	{
		Action = () => Console.Write(getValue());
	}
}