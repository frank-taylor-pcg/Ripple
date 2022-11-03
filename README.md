# Ripple
---
### A Resumable, Interruptable Process Language for C\#

This is an effort to create a high-level pseudo-language that runs within C# and can be interrupted at any time between execution of statements.  Goals for this pseudo-language are:

- Should be simple and intuitive for C# developers to use
- Should be robust and give meaningful error messages
- Should allow clear and concise development
- Should be easily extensible
- Should be interruptable - allowing the user to specify an event that will trigger the VM to stop operation between steps
- Should be resumable - once the interrupt has been handled, a separate event should allow the VM to resume from where it stopped

This is still in very early development and my language creation skills are rudimentary at best.  That said, don't expect to be overly impressed.

## Simplicity

In an effort to make this language as simple to use as possible I've borrowed most of the keywords from C# itself.  I am striving to ensure their functionality and usage is as familiar as possible as well.

The language contains the following keywords:
- Break
- Call (not yet implemented)
- Case
- CSAction
- Def (not yet implemented)
- Default
- Else
- ElseIf
- EndDef (not yet implemented)
- EndFor
- EndIf
- EndSwitch
- EndWhile
- For
- If
- Repeat
- Switch
- Until
- While


## Clear and Concise

By utilizing familiar syntax the learning curve for this should be fairly shallow.  This should allow developers to create clean solutions to fairly complicated problems.  Certain things had to be added because of the inability to use simple punctuation as part of the language.  Thus, statements that define a block such as `If`, `Switch`, `For`, etc. have matching `End` statements to mark the end of the block of code.  These are __ALWAYS__ required.

```csharp
// This is valid Ripple code:
vm
	.If(() => 1 == 1)
		.CSAction(() => Console.WriteLine("1 equals 1"))
	.EndIf();

vm.Run();

// This in NOT and will throw a validation error for the opening If statement
vm
	.If(() => 1 == 1)
		.CSAction(() => Console.WriteLine("1 equals 1"));

vm.Run();

```


## Extensibility

I'm still considering the best way to implement this. Currently each statement returns a reference to the VM so that they may be chained together, but all of the keywords are baked directly into the VM.  Adding new commands (i.e., domain-specific functions or keywords) can't be added without baking them into the VM as well.  I will need to think about this.


## Robustness

The first piece of this is that every command issued to the VM is wrapped in a try-catch.  This will ease a lot of the pain of development as error-handling is "baked in".  All exceptions are caught, wrapped in a special `RippleVMException` which supplies extra debugging information (C# line number, VM keyword and address, the actual C# expression in question), and then they are rethrown.  The hope is that the extra information will guide the developer to the specific piece of code that caused the issue.  As execution is deferred and programs can be loaded/unloaded (eventually) it may prove exceedingly difficult to debug without this information.

The next component is the Validator.  This is a process that runs behind the scenes when you call VirtualMachine.Run().  It steps through the code you have written, similar to a compiler, and reports errors that it detected.  Sadly, I have not yet figured out how to tie this into the C# build pipeline and this validation can only be done at runtime.


## Interruptable and resumable

The VM exposes a simple event that can be assigned for interrupting the VM's process flow.  Why would we do this?  The simplest example is if we're interacting with an external system that has a safety stop built in.  If we tie the VM's Interrupt event to a function monitoring the state of the safety stop, we can stop processing as soon as the stop has been triggered, preventing the software from attempting further operations, but without losing its state.  Once the safety stop is resolved, the system can be issued a Resume command and it can pick up where it left off.  _Note that this may not always be desirable and it may sometimes be better to restart or run a different process altogether._

## Examples

The following is a simple example of how to use Ripple:

```csharp
VirtualMachine vm = new();

string? name;

vm
	.CSAction(() => Console.Write("Please enter your name: "))
	.CSAction(() => name = Console.ReadLine())
	.CSAction(() => Console.WriteLine($"Hello, {name}"));

vm.Run();
```

The syntax is not as elegant as I would like, but it becomes fairly intuitive once you begin using it.  Variables are declared outside of the VM in the desired C# scope.  The `CSAction` command accepts a C# Action (hence the name) and runs it at a later time.  A great deal can be accomplished with this alone, but it may be difficult to see a use for this with such a simple example.

The following example showcases a bit more of the languages functionality:

```csharp
VirtualMachine vm = new();

// Variables used by the VM must be declared and have a valid value assigned prior to being referenced in the VM
int test = 0;

// Indentation isn't required, but can make reading your code easier
vm
	// The use of lambdas within the Switch statement is necessary to avoid premature evaluation of the test value
	.Switch(() => test)
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
	.EndSwitch();

for (test = 0; test < 3; test++)
{
	Console.WriteLine($"Running example with test = {test}");
	vm.Run();
}
```

In this example, you can see how the state of the VM can be influenced by changes at the outer C# layer.
