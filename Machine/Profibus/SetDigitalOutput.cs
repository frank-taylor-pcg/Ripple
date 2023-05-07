using Ripple.Statements;

namespace Machine.Profibus;

public class SetDigitalOutput : Statement
{
	public SetDigitalOutput(bool _on, ushort _address, int lineNumber = -1, string? expression = null)
		: base(lineNumber, expression)
	{
		Action = () => System.Diagnostics.Debug.WriteLine($"[SetDigitalOutput] => {expression}");
	}
}