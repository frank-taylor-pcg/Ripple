using Ripple;
using RippleTest;

bool isInterrupted = false;

// Allows us to simulate a simple interrupt from a user keypress event
bool GenerateInterruptFromUserInput()
{
	if (!Console.KeyAvailable) return isInterrupted;
	
	Console.ReadKey();
	isInterrupted = !isInterrupted;
	return isInterrupted;
}

VirtualMachine vm = new()
{
	Interrupt = GenerateInterruptFromUserInput
};

// The 'main' program
List<Test> tests = new()
{
	new Test("If block", TestFactory.TestIfBlock),
	new Test("Switch block", TestFactory.TestSwitchBlock),
	new Test("While loop", TestFactory.TestWhileBlock),
	new Test("Repeat-Until loop", TestFactory.TestRepeatBlock),
	new Test("For loop", TestFactory.TestForLoop),
	new Test("Interrupt test", TestFactory.TestInterrupt),
	new Test("Undeclared variable test", TestFactory.TestUndeclaredVariable),
	new Test("Custom library test", TestFactory.TestCustomLibrary)
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
