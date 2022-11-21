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

/*
Fixing chipper data issues
Fixing chipper Ok to Mill errors (probably some kind of physical issue)
Working on multi-cart, getting it ready for the "Don't switch o-plate on lims change" fix (or whatever that field is called)
Helping Neal figure out how to use the HD tray control I wrote
Keep getting pulled into meetings by Andrew Maddux (2 different ones this week, one that repeats daily...)
Started multi-cart testing
	found that UO1 is scanning the barcode on the cart itself (which it shouldn't) might be hardware (bad scanner ROI), might be software (bad position/offset calculation)
	Also found out that WMS Prod doesn't have the code to support multi-cart yet.  Alvaro reached out to Matt Beard to fast-track his PR (with my endorsement that it worked when I tested against it) so we're waiting on that

*/
