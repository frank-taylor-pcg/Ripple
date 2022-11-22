using Ripple;
using RippleTest;

bool isInterrupted = false;

// Allows us to simulate a simple interrupt from a user keypress event
bool GenerateInterruptFromUserInput()
{
	if (Console.KeyAvailable)
	{
		Console.ReadKey();
		isInterrupted = !isInterrupted;
	}
	return isInterrupted;
}

VirtualMachine vm = new()
{
	Interrupt = GenerateInterruptFromUserInput
};

// The 'main' program
List<Test> tests = new()
{
	new("If block", TestFactory.TestIfBlock),
	new("Switch block", TestFactory.TestSwitchBlock),
	new("While loop", TestFactory.TestWhileBlock),
	new("Repeat-Until loop", TestFactory.TestRepeatBlock),
	new("For loop", TestFactory.TestForLoop),
	new("Interrupt test", TestFactory.TestInterrupt),
	new("Custom library test", TestFactory.TestCustomLibrary),
};

int? userInput = null;
while (userInput != tests.Count)
{
	TestFramework.DisplayMenu(tests);
	userInput = TestFramework.GetUserInput();
	if (userInput != null)
	{
		TestFramework.ProcessUserInput((int)userInput, vm, tests);
	}
}
