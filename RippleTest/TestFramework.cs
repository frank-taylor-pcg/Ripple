using Ripple;

namespace RippleTest;

internal static class TestFramework
{
	internal static void RunTest(VirtualMachine vm, Test test)
	{
		Console.Clear();
		Logger.Banner($"Performing [{test.Name}] test");
		test.Action(vm);
		Logger.Log();
	}

	internal static void DisplayMenu(List<Test> tests)
	{
		Logger.Banner("Functionality tests:");
		int index = 0;
		foreach (Test test in tests)
		{
			Logger.Log($"  {index++}. {test.Name}");
		}
		Logger.Log($"  {index}. Quit");
		Logger.Log("Enter a number to select a test: ");
	}

	internal static int? GetUserInput()
	{
		string? input = Console.ReadLine();
		if (int.TryParse(input, out int result))
		{
			return result;
		}
		return null;
	}

	internal static void ProcessUserInput(int input, VirtualMachine vm, List<Test> tests)
	{
		bool isValid = input >= 0 && input <= tests.Count;
		if (isValid)
		{
			if (input < tests.Count)
				RunTest(vm, tests[input]);
		}
		else
		{
			Console.WriteLine($"Please enter an integer between 0 and {tests.Count}");
		}
	}
}