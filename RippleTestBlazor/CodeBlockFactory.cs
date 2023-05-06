using Ripple;

namespace RippleTestBlazor;

public static class CodeBlockFactory
{
	public static CodeBlock CreateIfBlock(Action<string> log)
	{
		CodeBlock cb = new();

		cb
			.DeclareVariable("Fruit", "apple")
			.If(() => "apple".Equals(cb.Mem.Fruit))
				.CsAction(() => log("  An apple a day keeps the doctor away!"))
			.ElseIf(() => "orange".Equals(cb.Mem.Fruit))
				.CsAction(() => log("  What rhymes with orange?"))
			.Else()
				.CsAction(() => log($"  You entered {cb.Mem.Fruit}"))
			.EndIf();
		
		return cb;
	}
	
	public static CodeBlock CreateSwitchBlock(Action<string> log)
	{
		CodeBlock cb = new();

		cb
			.DeclareVariable("Choice", 10)
			
			.Switch(() => cb.Mem.Choice)
				.Case(0)
					.CsAction(() => log($"  Without a Break this falls through : Choice = {cb.Mem.Choice}"))
				.Case(1)
				.Case(2)
				.Case(3)
				.Case(5)
				.Case(6)
				.Case(7)
					.CsAction(() => log($"  Case 0, 1, 2, 3, 5, 6, or 7 triggered : Choice = {cb.Mem.Choice}"))
					.Break()
				.Default()
					.CsAction(() => log($"  Default triggered : Choice = {cb.Mem.Choice}"))
				.Break()
			.EndSwitch();

		return cb;
	}
	
	public static CodeBlock CreateWhileBlock(Action<string> log)
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

				.CsAction(() => log(cb.Mem.Line))

			.EndWhile();

		return cb;
	}

	public static CodeBlock CreateRepeatBlock(Action<string> log)
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

				.CsAction(() => log(cb.Mem.Line))

			.Until(() => cb.Mem.I! == cb.Mem.MaxIndex!);

		return cb;
	}

	public static CodeBlock CreateForBlock(Action<string> log)
	{
		CodeBlock cb = new();

		cb
			.DeclareVariable("I", 0)
			.For(() => cb.Mem.I < 10, () => cb.Mem.I++)
				.CsAction(() => log($"For loop index = {cb.Mem.I}"))
			.EndFor();

		return cb;
	}

	public static CodeBlock CreateLongRunningProcess(Action<string> log)
	{
		CodeBlock cb = new();

		cb
			.DeclareVariable("I", 0)
			.CsAction(() => log("Press any key to interrupt this very long process"))
			.For(() => cb.Mem.I < 100, () => cb.Mem.I++)
				.CsAction(() => log("."))
			.EndFor();

		return cb;
	}

	/*
	public static CodeBlock CreateCustomLibraryTest(Action<string> log)
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
			.Iterate(log, (IEnumerable<object>)cb.Mem.Names);

		return cb;
	}
*/
	public static CodeBlock CreateUndeclaredVariableTest(Action<string> log)
	{
		CodeBlock cb = new();

		cb
			.CsAction(() => cb.Mem.MyUndeclaredVariable = 20)
			.CsAction(() => log("If developer opted to allow implicit variable usage this will work without warning"))
			.CsAction(() => log("Otherwise, an exception will be thrown during validation"))
			.CsAction(() => log($"The value of MyUndeclaredVariable is {cb.Mem.MyUndeclaredVariable}"));

		return cb;
	}

}