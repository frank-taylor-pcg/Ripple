using Ripple.Statements;

namespace RippleTest.ExampleLib;

/// <summary>
/// An example of an added keyword that performs the supplied action on all elements of
/// a collection in a single cycle of the virtual machine.
/// </summary>
internal class Iterate : Statement
{

	public Iterate(Action act, int lineNumber = -1, string? expression = null)
		: base(lineNumber, expression)
	{
		Action = act;
	}

	public static Iterate Create<T>(
		Action<T> act
		, IEnumerable<T> values
		, int lineNumber = -1
		, string? expression = null)
	{

		Iterate result = new(() =>
			{
				foreach (T value in values)
				{
					act(value);
				}
			}
			, lineNumber, expression);

		return result;
	}
}