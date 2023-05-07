using Ripple;
using System.Runtime.CompilerServices;

namespace Machine.Profibus;

public static class Api
{
	public static CodeBlock SetVacuumPumpOn(
		this CodeBlock cb
		, bool on
		, ushort pumpAddress
		, [CallerLineNumber] int lineNumber = -1)
	{
		string expression = $"{nameof(SetVacuumPumpOn)}(on: {on}, pumpAddress: {pumpAddress})";
		cb.Add(new SetDigitalOutput(on, pumpAddress, lineNumber, expression));
		return cb;
	}

	public static CodeBlock GetVacuumPumpState(
		this CodeBlock cb
		, ushort pumpAddress
		, string variableName
		, [CallerLineNumber] int lineNumber = -1)
	{
		string expression = $"{nameof(GetVacuumPumpState)}(pumpAddress: {pumpAddress}, result: {variableName})";
		cb.Add(new GetDigitalOutput(pumpAddress, variableName, lineNumber, expression));
		return cb;
	}
}