using Ripple.Statements;

namespace RippleTest.ExampleLib
{
	/// <summary>
	/// An example of an added keyword that creates a 'banner' using the supplied text value and writes it to the console.
	/// The use of a Func to get the string value allows it to be evaluated when this command is called, not when it is
	/// created.
	/// </summary>
	internal class Banner : Statement
	{
		public Banner(Func<string> getValue, int width = 40, int lineNumber = -1, string? expression = null)
				: base(lineNumber, expression)
		{
			Action = () =>
			{
				Console.WriteLine("".PadLeft(width, '-'));
				Console.WriteLine($"- {getValue()}");
				Console.WriteLine("".PadLeft(width, '-'));
			};
		}
	}
}
