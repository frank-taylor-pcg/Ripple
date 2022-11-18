using Ripple;
using RippleTest;
using RippleTest.Tests;

VirtualMachine vm = new();

void TestIfBlock()
{
	CodeBlock cb = TestPrograms.CreateIfTest();
	List<string> fruits = new() { "apple", "orange", "banana", "stapler" };
	vm.CodeBlock = cb;
	foreach (string fruit in fruits)
	{
		vm.CodeBlock.Mem.Fruit = fruit;
		vm.Run();
	}
}

void TestSwitchBlock()
{
	vm.CodeBlock = TestPrograms.CreateSwitchTest();
	for (int i = 0; i < 10; i++)
	{
		vm.CodeBlock.Mem.Choice = i;
		vm.Run();
	}
}

void TestWhileBlock()
{
	vm.CodeBlock = TestPrograms.CreateWhileTest();
	vm.Run();
}

void TestRepeatBlock()
{
	vm.CodeBlock = TestPrograms.CreateRepeatTest();
	vm.Run();
}

void TestForLoop()
{
	vm.CodeBlock = TestPrograms.CreateForTest();
	vm.Run();
}

void RunTest(Test test)
{
	Console.Clear();
	Logger.Banner($"Performing [{test.Name}] test");
	test.Action();
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
	new("Switch block", TestSwitchBlock),
	new("While loop", TestWhileBlock),
	new("Repeat-Until loop", TestRepeatBlock),
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
