namespace Ripple
{
	internal class Logger
	{
		public static void Log(string message)
		{
			Console.WriteLine(message);
			//System.Diagnostics.Debug.WriteLine(message);
		}

		public static void Log(object? obj = null)
		{
			Log($"{obj}");
		}

		public static void Banner(object? obj = null)
		{
			Log("".PadLeft(40, '-'));
			Log($"- {obj}");
			Log("".PadLeft(40, '-'));
		}
	}
}
