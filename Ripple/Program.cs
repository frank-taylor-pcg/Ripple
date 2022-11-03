using Ripple;
using System.Text;

void TestIfBlock(bool verbose)
{
	VirtualMachine vm = new() { Verbose = verbose };

	object? fruit = null;

	vm
		.If(() => "apple".Equals(fruit))
			.CSAction(() => Logger.Log($"  If condition was true : fruit = {fruit}"))
		.ElseIf(() => "orange".Equals(fruit))
		//.CSAction(() => Logger.Log($"  ElseIf condition was true : fruit = {fruit}"))
		.Else()
			.CSAction(() => Logger.Log($"  Else condition was true : fruit = {fruit}"))
		.EndIf();

	List<object> testValues = new() { "apple", "orange", 2, "banana" };

	foreach (object s in testValues)
	{
		Logger.Log($"Running {nameof(TestIfBlock)} with test = {s}");
		fruit = s;
		vm.Run();
	}
}

void TestBasicSwitchBlock(bool verbose)
{
	VirtualMachine vm = new() { Verbose = verbose };
	int test = 0;
	vm
		.Switch(() => test)
			.Case(0)
				.CSAction(() => Logger.Log($"  Case 0 triggered."))
				.Break()
			.Case(1)
				.CSAction(() => Logger.Log($"  Case 1 triggered"))
				.Break()
			.Default()
				.CSAction(() => Logger.Log($"  Default triggered : test = {test}"))
				.Break()
		.EndSwitch();

	for (test = 0; test < 3; test++)
	{
		Logger.Log($"Running {nameof(TestBasicSwitchBlock)} with test = {test}");
		vm.Run();
	}
}

void TestCaseFallThrough(bool verbose)
{
	VirtualMachine vm = new() { Verbose = verbose };
	int test = 0;

	vm
		.Switch(() => test)
			.Case(0)
				.CSAction(() => Logger.Log($"  Without a Break this falls through : test = {test}"))
			.Case(1)
			.Case(2)
			.Case(3)
			.Case(5)
			.Case(6)
			.Case(7)
				.CSAction(() => Logger.Log($"  Case 0, 1, 2, 3, 5, 6, or 7 triggered : test = {test}"))
				.CSAction(() => test++)
				.Break()
			.Default()
				.CSAction(() => Logger.Log($"  Default triggered : test = {test}"))
				.CSAction(() => test++)
		.EndSwitch();

	for (int i = 0; i < 10; i++)
	{
		Logger.Log($"Running {nameof(TestCaseFallThrough)} with test = {i}");
		test = i;
		vm.Run();
	}
}

void TestWhileLoop(bool verbose)
{
	int i = 0;
	char c = 'a';
	StringBuilder sb = new();
	bool shouldInterrupt = false;

	VirtualMachine vm = new(0)
	{
		Verbose = verbose,
		Interrupt = () => { return shouldInterrupt; }
	};


	// The inner loop is broken
	vm
		.While(() => i < 10)
			.CSAction(() => sb.Append(i))
			.CSAction(() => i++)

			.CSAction(() => c = 'a')

			.While(() => c < 'c')
				.CSAction(() => sb.Append(c))
				.CSAction(() => c++)
			.EndWhile()

			.CSAction(() => shouldInterrupt = true)

		.EndWhile()
		.CSAction(() => Logger.Log(sb));

	vm.Run();
}

void TestRepeatLoop(bool verbose)
{
	VirtualMachine vm = new() { Verbose = verbose };

	int i = 0;
	char c = 'a';

	vm
		.Repeat()
			.CSAction(() => Logger.Log($"i = {i}"))
			.CSAction(() => i++)

			.CSAction(() => c = 'a')

			.Repeat()
				.CSAction(() => Logger.Log($"  c = {c}"))
				.CSAction(() => c++)
		.Until(() => c == 'c')

		.Until(() => i == 10);

	vm.Run();
}

void TestForLoop(bool verbose)
{
	VirtualMachine vm = new() { Verbose = verbose };

	int i = 0;

	vm
		.For(() => i < 10, () => i++)
			.CSAction(() => Logger.Log($"i = {i}"))
		.EndFor();

	vm.Run();
}

void RunTest(Test test)
{
	Logger.Banner($"Performing [{test.Name}] test");
	test.Action(false);
}

void DisplayMenu(List<Test> tests)
{
	Logger.Banner("Functionality tests:");
	int index = 0;
	foreach (Test test in tests)
	{
		Logger.Log($"  {index++}. {test.Name}");
	}
	Logger.Log($"  {index}. Quit");
	Logger.Log($"Enter a number to select a test: ");
}

int? GetUserInput()
{
	string? input = Console.ReadLine();
	if (int.TryParse(input, out int result))
	{
		return result;
	}
	return null;
}

void ProcessInput(int input, List<Test> tests)
{
	bool isValid = input >= 0 && input <= tests.Count;
	if (isValid)
	{
		if (input < tests.Count)
			RunTest(tests[input]);
	}
	else
	{
		Console.WriteLine($"Please enter an integer between 0 and {tests.Count}");
	}
}

// The 'main' program
List<Test> tests = new()
{
	new("If block", TestIfBlock),
	new("Switch block (basic)", TestBasicSwitchBlock),
	new("Switch block (case-fallthrough)", TestCaseFallThrough),
	new("While loop", TestWhileLoop),
	new("Repeat-Until loop", TestRepeatLoop),
	new("For loop", TestForLoop),
};

int? userInput = null;
while (userInput != tests.Count)
{
	DisplayMenu(tests);
	userInput = GetUserInput();
	if (userInput != null)
	{
		ProcessInput((int)userInput, tests);
	}
}

internal class Test
{
	public string Name { get; set; }
	public Action<bool> Action { get; set; }
	public Test(string name, Action<bool> action)
	{
		Name = name;
		Action = action;
	}
}