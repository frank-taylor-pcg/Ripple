using Ripple.Statements;

namespace Machine.Profibus;

public class GetDigitalOutput : Statement
{
	public GetDigitalOutput(ushort _address, string variableName, int lineNumber = -1, string? expression = null)
		: base(lineNumber, expression)
	{
		Action = () =>
		{
			Vm!.CodeBlock!.SetVariable(variableName, true);
			System.Diagnostics.Debug.WriteLine($"[GetDigitalOutput] => {expression}");
		};
	}
}