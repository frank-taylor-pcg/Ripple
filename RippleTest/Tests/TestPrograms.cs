using Ripple;
using Ripple.Keywords;

namespace RippleTest.Tests
{
	public class TestPrograms
	{
		public static CodeBlock CreateIfTest()
		{
			CodeBlock cb = new();

			cb
			.DeclareVariable("Fruit", "apple")

			.If(() => "apple".Equals(cb.Mem.Fruit))
				.CSAction(() => Logger.Log($"  An apple a day keeps the doctor away!"))

			.ElseIf(() => "orange".Equals(cb.Mem.Fruit))
				.CSAction(() => Logger.Log($"  What rhymes with orange?"))

			.Else()
				.CSAction(() => Logger.Log($"  You entered {cb.Mem.Fruit}"))

			.EndIf();

			return cb;
		}

		public static CodeBlock CreateSwitchTest()
		{
			CodeBlock cb = new();

			cb
			.DeclareVariable("Choice", 10)

			.Switch(() => cb.Mem.Choice)
				.Case(0)
					.CSAction(() => Logger.Log($"  Without a Break this falls through : Choice = {cb.Mem.Choice}"))
				.Case(1)
				.Case(2)
				.Case(3)
				.Case(5)
				.Case(6)
				.Case(7)
					.CSAction(() => Logger.Log($"  Case 0, 1, 2, 3, 5, 6, or 7 triggered : Choice = {cb.Mem.Choice}"))
					.Break()
				.Default()
					.CSAction(() => Logger.Log($"  Default triggered : Choice = {cb.Mem.Choice}"))
					.Break()
			.EndSwitch();

			return cb;
		}

		// The variables are getting really ugly. A double-cast just to increment a char is too much.
		public static CodeBlock CreateWhileTest()
		{
			CodeBlock cb = new();
			cb
			.DeclareVariable("I", 0)
			.DeclareVariable("MaxIndex", 10)

			.DeclareVariable("C", 'a')
			.DeclareVariable("MaxCharIndex", 'd')

			.DeclareVariable("Line", string.Empty)

			.While(() => cb.Mem.I! < cb.Mem.MaxIndex!)

				.CSAction(() => cb.Mem.Line = $"{cb.Mem.I} ")
				.CSAction(() => cb.Mem.I = cb.Mem.I += 1)

				.CSAction(() => cb.Mem.C = 'a')

				.While(() => cb.Mem.C! < cb.Mem.MaxCharIndex!)
					.CSAction(() => cb.Mem.Line += $"{cb.Mem.C} ")
					.CSAction(() => cb.Mem.C = (char)(cb.Mem.C += 1))
				.EndWhile()

				.CSAction(() => Logger.Log(cb.Mem.Line))

			.EndWhile();

			return cb;
		}

		public static CodeBlock CreateRepeatTest()
		{
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
				.CSAction(() => cb.Mem.D = "How can I prevent this? Do I even want to?")
				.CSAction(() => cb.Mem.E = "Perhaps the BlockValidator could warn the user if")
				.CSAction(() => cb.Mem.F = "a variable is used before being declared")

				.Repeat()
					.CSAction(() => cb.Mem.Line += $"{cb.Mem.C} ")
					.CSAction(() => cb.Mem.C = (char)(cb.Mem.C += 1))
				.Until(() => cb.Mem.C! == cb.Mem.MaxCharIndex!)

				.CSAction(() => Logger.Log(cb.Mem.Line))

			.Until(() => cb.Mem.I! == cb.Mem.MaxIndex!);

			return cb;
		}

		public static CodeBlock CreateForTest()
		{
			CodeBlock cb = new();

			cb
			.DeclareVariable("I", 0)
			.For(() => cb.Mem.I < 10, () => cb.Mem.I++)
				.CSAction(() => Logger.Log($"For loop index = {cb.Mem.I}"))
			.EndFor();

			return cb;
		}
	}
}
