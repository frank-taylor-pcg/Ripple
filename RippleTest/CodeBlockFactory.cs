using Ripple;
using RippleTest.ExampleLib;

namespace RippleTest;

public static class CodeBlockFactory
{
	public static CodeBlock CreateIfBlock()
	{
		CodeBlock cb = new();

		cb
			.DeclareVariable("Fruit", "apple")

			.If(() => "apple".Equals(cb.Mem.Fruit))
			.CsAction(() => Logger.Log("  An apple a day keeps the doctor away!"))

			.ElseIf(() => "orange".Equals(cb.Mem.Fruit))
			.CsAction(() => Logger.Log("  What rhymes with orange?"))

			.Else()
			.CsAction(() => Logger.Log($"  You entered {cb.Mem.Fruit}"))

			.EndIf();

		return cb;
	}

	public static CodeBlock CreateSwitchBlock()
	{
		CodeBlock cb = new();

		cb
			.DeclareVariable("Choice", 10)

			.Switch(() => cb.Mem.Choice)
			.Case(0)
			.CsAction(() => Logger.Log($"  Without a Break this falls through : Choice = {cb.Mem.Choice}"))
			.Case(1)
			.Case(2)
			.Case(3)
			.Case(5)
			.Case(6)
			.Case(7)
			.CsAction(() => Logger.Log($"  Case 0, 1, 2, 3, 5, 6, or 7 triggered : Choice = {cb.Mem.Choice}"))
			.Break()
			.Default()
			.CsAction(() => Logger.Log($"  Default triggered : Choice = {cb.Mem.Choice}"))
			.Break()
			.EndSwitch();

		return cb;
	}

	public static CodeBlock CreateWhileBlock()
	{
		CodeBlock cb = new();
		cb
			.DeclareVariable("I", 0)
			.DeclareVariable("MaxIndex", 10)

			.DeclareVariable("C", 'a')
			.DeclareVariable("MaxCharIndex", 'd')

			.DeclareVariable("Line", string.Empty)

			.While(() => cb.Mem.I! < cb.Mem.MaxIndex!)

			.CsAction(() => cb.Mem.Line = $"{cb.Mem.I} ")
			.CsAction(() => cb.Mem.I = cb.Mem.I += 1)

			.CsAction(() => cb.Mem.C = 'a')

			.While(() => cb.Mem.C! < cb.Mem.MaxCharIndex!)
			.CsAction(() => cb.Mem.Line += $"{cb.Mem.C} ")
			.CsAction(() => cb.Mem.C = (char)(cb.Mem.C += 1))
			.EndWhile()

			.CsAction(() => Logger.Log(cb.Mem.Line))

			.EndWhile();

		return cb;
	}

	public static CodeBlock CreateRepeatBlock()
	{
		CodeBlock cb = new();

		cb
			.DeclareVariable("I", 0)
			.DeclareVariable("MaxIndex", 10)

			.DeclareVariable("C", 'a')
			.DeclareVariable("MaxCharIndex", 'd')

			.DeclareVariable("Line", string.Empty)

			.Repeat()

			.CsAction(() => cb.Mem.Line = $"{cb.Mem.I} ")
			.CsAction(() => cb.Mem.I = cb.Mem.I += 1)

			.CsAction(() => cb.Mem.C = 'a')
			.CsAction(() => cb.Mem.D = "How can I prevent this? Do I even want to?")
			.CsAction(() => cb.Mem.E = "Perhaps the BlockValidator could warn the user")
			.CsAction(() => cb.Mem.F = "if a variable is used before being declared")

			.Repeat()
			.CsAction(() => cb.Mem.Line += $"{cb.Mem.C} ")
			.CsAction(() => cb.Mem.C = (char)(cb.Mem.C += 1))
			.Until(() => cb.Mem.C! == cb.Mem.MaxCharIndex!)

			.CsAction(() => Logger.Log(cb.Mem.Line))

			.Until(() => cb.Mem.I! == cb.Mem.MaxIndex!);

		return cb;
	}

	public static CodeBlock CreateForBlock()
	{
		CodeBlock cb = new();

		cb
			.DeclareVariable("I", 0)
			.For(() => cb.Mem.I < 10, () => cb.Mem.I++)
			.CsAction(() => Logger.Log($"For loop index = {cb.Mem.I}"))
			.EndFor();

		return cb;
	}

	public static CodeBlock CreateLongRunningProcess()
	{
		CodeBlock cb = new();

		cb
			.DeclareVariable("I", 0)
			.CsAction(() => Console.WriteLine("Press any key to interrupt this very long process"))
			.For(() => cb.Mem.I < 100, () => cb.Mem.I++)
			.CsAction(() => Console.Write("."))
			.EndFor();

		return cb;
	}

	public static CodeBlock CreateCustomLibraryTest()
	{
		CodeBlock cb = new();

		cb
			.DeclareVariable("Names", new List<string> { "Frank", "Ripple", "World" })
			.DeclareVariable("I", 0)
			.DeclareVariable("Name", string.Empty)

			.Banner(() => "Performing the greetings")

			.ForEach("Name", (IEnumerable<object>)cb.Mem.Names)
			.WriteLine(() => $"Hello, {cb.Mem.Name}")
			.EndForEach()

			.Banner(() => "Saying goodbye")

			.ForEach("Name", (IEnumerable<object>)cb.Mem.Names)
			.WriteLine(() => $"Goodbye, {cb.Mem.Name}")
			.EndForEach()

			.Banner(() => "We said hello and goodbye to the following:")
			.Iterate(Console.WriteLine, (IEnumerable<object>)cb.Mem.Names);

		return cb;
	}

	public static CodeBlock CreateUndeclaredVariableTest()
	{
		CodeBlock cb = new();

		cb
			.CsAction(() => cb.Mem.MyUndeclaredVariable = 20)
			.CsAction(() => Console.WriteLine("Developer opted to allow implicit variable usage."))
			.CsAction(() => Console.WriteLine($"The value of MyUndeclaredVariable is {cb.Mem.MyUndeclaredVariable}"));

		return cb;
	}
}