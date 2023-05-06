using System.Runtime.CompilerServices;

namespace Ripple.Exceptions;

public static class Guards
{
	public static void ThrowIfNull(object? argument, [CallerArgumentExpression("argument")] string? argumentName = null)
	{
		if (argument is null)
		{
			throw new NullReferenceException($"{argumentName} was null");
		}
	}
}