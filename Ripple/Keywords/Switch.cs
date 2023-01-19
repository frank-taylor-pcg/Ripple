using Ripple.Statements;
using Ripple.Validators;

namespace Ripple.Keywords
{
	public class Switch : BlockStatement, IBlockParent
	{
		public readonly Func<object> Value;

		public Switch(Func<object> getValue, int lineNumber, string? expression) : base(lineNumber, expression)
		{
			Value = getValue;
		}

		public void ConstructBlock(List<Statement> statements, int startAddress)
		{
			if (statements[startAddress] is not Switch @switch) return;

			int address = startAddress + 1;

			// Once we encounter a Default we can't add any more Case or Default statements for this block
			bool endSwitchExpected = false;

			@switch.Block = new() { Parent = @switch };

			while (address < statements.Count && !@switch.Block.IsValid)
			{
				if (!endSwitchExpected && statements[address] is Case @case)
				{
					@switch.Block.AddJumpTarget(@case);
				}

				if (!endSwitchExpected && statements[address] is Default @default)
				{
					@switch.Block.AddJumpTarget(@default);
					endSwitchExpected = true;
				}

				if (statements[address] is EndSwitch endSwitch)
				{
					@switch.Block.AddJumpTarget(endSwitch);
					@switch.Block.IsValid = true;
				}

				if (statements[address] is Break @break)
				{
					// Break statements are not jump targets, but need a reference to the block they belong to
					@break.Block = @switch.Block;
				}

				address++;
			}
		}

		public override bool IsValid() => Block!.IsValid;
	}
}
