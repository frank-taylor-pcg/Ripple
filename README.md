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
- EndForEach
- EndIf
- EndSwitch
- EndWhile
- For
- ForEach
- If
- Repeat
- Switch
- Until
- While

## Clear and Concise

By utilizing familiar syntax the learning curve for this should be fairly shallow.  This should allow developers to create clean solutions to fairly complicated problems.  Certain things had to be added because of the inability to use simple punctuation as part of the language.  Thus, statements that define a block such as `If`, `Switch`, `For`, etc. have matching `End` statements to mark the end of the block of code.  These are __ALWAYS__ required.

```csharp
// This is valid Ripple code:
CodeBlock cbValid = new();

cbValid
.If(() => 1 == 1)
	.CSAction(() => Console.WriteLine("1 equals 1"))
.EndIf();

vm.CodeBlock = cbValid;
vm.Run();

// This in NOT and will throw a validation error for the opening If statement
CodeBlock cbInvalid = new();

cbInvalid
.If(() => 1 == 1)
	.CSAction(() => Console.WriteLine("1 equals 1"));

vm.CodeBlock = cbInvalid;
vm.Run();
```

## Extensibility

New keywords and functionality are easy to add.  For each new keyword a class must be defined that derives from `Statement` or `BlockStatement`.  For simple Statements, definition of the action they perform is all that is required, but extra validation can be performed by overriding `IsValid`.  BlockStatements are more difficult.  At a minimum there must be two statements for any block.  The parent statement and the End statement.  The parent `BlockStatement` must implement the `IBlockParent` interface.  This interface exposes the `ConstructBlock` method which steps through the list of Statements in the `CodeBlock` and attempts to build a valid block.  If this fails, the code will not run and a `CodeValidationException` will be thrown.  Note that for new looping constructs, the `LoopConstructor` is available to simplify the construction and validation process.

## Robustness

The first piece of this is that every command issued to the VM is wrapped in a try-catch.  This will ease a lot of the pain of development as error-handling is "baked in".  All exceptions are caught, wrapped in a special `RippleVMException` which supplies extra debugging information (C# line number, VM keyword and address, the actual C# expression in question), and then they are rethrown.  The hope is that the extra information will guide the developer to the specific piece of code that caused the issue.  As execution is deferred and CodeBlocks can be loaded/unloaded it may prove exceedingly difficult to debug without this information.

The next component is the validator.  This is a process that runs behind the scenes when you call `VirtualMachine.Run()`.  It steps through the code you have written, similar to a compiler, and reports errors that it detected.  Sadly, I have not yet figured out how to tie this into the C# build pipeline and this validation can only be done at runtime.


## Interruptable and resumable

The VM exposes a simple event, `Interrupt` that can be assigned for interrupting the VM's process flow.  This allows us to interact with external processes that may need to be synchronized with the state of our program.  By interrupting our VM, we can essentially pause it until the point that the external connection is ready for it to continue.  _Note that this may not always be desirable and it may sometimes be better to restart or run a different process altogether._  Sometimes the VM may be halted due to an error.  If this happens and you need to debug the code, the VM has the ability to dump the state of the currently loaded CodeBlock.  Below is an example of such a dump:

```
----------------------------------------
- Performing [Repeat-Until loop] test
----------------------------------------
0 a b c
1 a b c
2 a b c
3 a b c
4 a b c

Process interrupted. Current VM state:
  Halted during execution of [VM/C#   5/  99] : Repeat     (Block ID: d1ef3e)

Code Listing:
  [VM/C#   0/  91] : DeclareVariable "I" = 0
  [VM/C#   1/  92] : DeclareVariable "MaxIndex" = 10
  [VM/C#   2/  94] : DeclareVariable "C" = 'a'
  [VM/C#   3/  95] : DeclareVariable "MaxCharIndex" = 'd'
  [VM/C#   4/  97] : DeclareVariable "Line" = string.Empty
  [VM/C#   5/  99] : Repeat     (Block ID: d1ef3e)
  [VM/C#   6/ 101] : CSAction () => cb.Mem.Line = $"{cb.Mem.I} "
  [VM/C#   7/ 102] : CSAction () => cb.Mem.I = cb.Mem.I += 1
  [VM/C#   8/ 104] : CSAction () => cb.Mem.C = 'a'
  [VM/C#   9/ 105] : CSAction () => cb.Mem.D = "How can I prevent this? Do I even want to?"
  [VM/C#  10/ 106] : CSAction () => cb.Mem.E = "Perhaps the BlockValidator could warn the user"
  [VM/C#  11/ 107] : CSAction () => cb.Mem.F = "if a variable is used before being declared"
  [VM/C#  12/ 109] : Repeat     (Block ID: d5fcb1)
  [VM/C#  13/ 110] : CSAction () => cb.Mem.Line += $"{cb.Mem.C} "
  [VM/C#  14/ 111] : CSAction () => cb.Mem.C = (char)(cb.Mem.C += 1)
  [VM/C#  15/ 112] : Until () => cb.Mem.C! == cb.Mem.MaxCharIndex!    (Block ID: d5fcb1)
  [VM/C#  16/ 114] : CSAction () => Logger.Log(cb.Mem.Line)
  [VM/C#  17/ 116] : Until () => cb.Mem.I! == cb.Mem.MaxIndex!    (Block ID: d1ef3e)

Memory Dump:
  I : 5
  MaxIndex : 10
  C : d
  MaxCharIndex : d
  Line : 4 a b c
  D : How can I prevent this? Do I even want to?
  E : Perhaps the BlockValidator could warn the user
  F : if a variable is used before being declared
```
Notice that it gave us everything about the state of the currently running CodeBlock from:
- The line of code the VM halted on - complete with the VM memory address, actual C# line number, and a fairly close approximation of the actual code written
- The entire code listing for the loaded CodeBlock
- The current value of every variable in the CodeBlock (even those created improperly)
- Block ID values to help determine if the code was constructed correctly

## Examples

The following is a simple example of how to use Ripple:

```csharp
VirtualMachine vm = new();
CodeBlock cb = new();
string? name;

cb
.CSAction(() => Console.Write("Please enter your name: "))
.CSAction(() => name = Console.ReadLine())
.CSAction(() => Console.WriteLine($"Hello, {name}"));

vm.CodeBlock = cb;
vm.Run();
```

The syntax is not as elegant as I would like, but it becomes fairly intuitive once you begin using it.  Variables can be declared outside of the VM in the desired C# scope and then accessed from within the CodeBlock if both are defined in the same parent scope.  The `CSAction` command accepts a C# Action (hence the name) and runs it at a later time.  A great deal can be accomplished with this alone, but it may be difficult to see a use for this with such a simple example.

The following example showcases a bit more of the languages functionality:

```csharp
VirtualMachine vm = new();
CodeBlock cb = new();
int test = 0;

// Indentation isn't required, but can make reading your code easier
cb
// The use of lambdas within the Switch statement is necessary to avoid premature evaluation of the test value
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

vm.CodeBlock = cb;

for (test = 0; test < 3; test++)
{
	Console.WriteLine($"Running example with test = {test}");
	vm.Run();
}
```

In the above example, you can see how the state of the VM can be influenced by changes at the outer C# layer.  Sometimes though, we'll need to define a code segment that tracks its internal state.  Below we define a CodeBlock that defines a number of variables and then uses them in a pair of nested loops.  There is probably room for improvement here.  Currently to declare a variable you call the DeclareVariable function passing the variable name and it's starting value.  The initial value will allow the virtual machine to determine the data type.  To access the variables you must use the notation `<codeblock name>.Mem.<variable name>`.  This is possible by using the [ExpandoObject](learn.microsoft.com/en-us/dotnet/api/system.dynamic.expandoobject) class in the [System.Dynamic](learn.microsoft.com/en-us/dotnet/api/system.dynamic) namespace.  This gives us a great deal of flexibility, but comes at the cost of safety.  It is possible to reference a variable that has not yet been declared or to define new variables outside of the intended method.  Both of these could potentially cause issues.

**Note that all variables are global to the current CodeBlock.**

```csharp
VirtualMachine vm = new();
CodeBlock cb = new();

cb
.DeclareVariable("I", 0)
.DeclareVariable("MaxIndex", 10)

.DeclareVariable("C", 'a')
.DeclareVariable("MaxCharIndex", 'd')

.DeclareVariable("Line", string.Empty)

.Repeat()

	.CSAction(() => cb.Mem.Line = $"{cb.Mem.I} ")
	.CSAction(() => cb.Mem.I = cb.Mem.I += 1)

	.CSAction(() => cb.Mem.C = 'a')
	.Repeat()
		.CSAction(() => cb.Mem.Line += $"{cb.Mem.C} ")
		.CSAction(() => cb.Mem.C = (char)(cb.Mem.C += 1))
	.Until(() => cb.Mem.C! == cb.Mem.MaxCharIndex!)

	.CSAction(() => Logger.Log(cb.Mem.Line))

.Until(() => cb.Mem.I! == cb.Mem.MaxIndex!);

vm.CodeBlock = cb;
vm.Run();
```

For more examples in how to utilize Ripple, please examine the RippleTest application.