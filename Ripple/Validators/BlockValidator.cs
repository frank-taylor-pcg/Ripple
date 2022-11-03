using Ripple.Exceptions;
using Ripple.Keywords;
using Ripple.Statements;

namespace Ripple.Validators
{
	internal static class BlockValidator
	{
		private static List<string> ValidationErrors = new();

		public static void Validate(VirtualMachine vm)
		{
			// Pass 1: Attempt to build each block
			ConstructBlockStatements(vm);

			// Pass 2: Step through the entire code listing and ensure they're all valid, if not log the validation error
			ReportValidationErrors(vm);
		}

		private static void ConstructBlockStatements(VirtualMachine vm)
		{
			foreach (Statement statement in vm.Statements)
			{
				if (statement is If @if)
				{
					ConstructIfBlock(vm.Statements, @if.Address);
				}

				if (statement is Switch @switch)
				{
					ConstructSwitchBlock(vm.Statements, @switch.Address);
				}

				if (statement is While @while)
				{
					ConstructGenericLoop(vm.Statements, @while.Address, typeof(While), typeof(EndWhile));
				}

				if (statement is Repeat @repeat)
				{
					ConstructGenericLoop(vm.Statements, @repeat.Address, typeof(Repeat), typeof(Until));
				}

				if (statement is For @for)
				{
					ConstructGenericLoop(vm.Statements, @for.Address, typeof(For), typeof(EndFor));
				}
			}
		}

		private static void ReportValidationErrors(VirtualMachine vm)
		{
			ValidationErrors = new();
			foreach (Statement statement in vm.Statements)
			{
				if (!statement.IsValid())
				{
					string? msg = statement.GetValidationErrorMessage();
					if (msg != null)
						ValidationErrors.Add(msg);
				}
			}

			if (ValidationErrors.Count > 0)
			{
				throw new CodeValidationException(ValidationErrors);
			}
			else
			{
				vm.CodeIsValid = true;
			}
		}

		public static void ConstructIfBlock(List<Statement> statements, int startAddress)
		{
			if (statements[startAddress] is If @if)
			{
				int address = startAddress + 1;

				// Once we encounter an Else we can't add any more ElseIf or Else statements for this block
				bool endifExpected = false;

				@if.Block = new() { Parent = @if };

				while (address < statements.Count && !@if.Block.IsValid)
				{
					if (!endifExpected && statements[address] is ElseIf elseif)
					{
						@if.Block.AddJumpTarget(elseif);
					}

					if (!endifExpected && statements[address] is Else @else)
					{
						@if.Block.AddJumpTarget(@else);
						endifExpected = true;
					}

					if (statements[address] is EndIf endif)
					{
						@if.Block.AddJumpTarget(endif);
						@if.Block.IsValid = true;
					}

					address++;
				}
			}
		}

		public static void ConstructSwitchBlock(List<Statement> statements, int startAddress)
		{
			if (statements[startAddress] is Switch @switch)
			{
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
		}

		private static int ConstructGenericLoop(List<Statement> statements, int startAddress, Type openType, Type closeType)
		{
			int address = startAddress;

			if (statements[startAddress].GetType().Equals(openType))
			{
				address++;
				BlockStatement? loopStart = statements[startAddress] as BlockStatement;

				loopStart!.Block = new() { Parent = loopStart };

				while (address < statements.Count && !loopStart.Block.IsValid)
				{
					if (statements[address].GetType().Equals(closeType))
					{
						BlockStatement? loopEnd = statements[address] as BlockStatement;
						loopStart.Block.AddJumpTarget(loopEnd!);
						loopStart.Block.IsValid = true;
					}

					// First attempt at nesting
					if (statements[address].GetType().Equals(openType))
					{
						address = ConstructGenericLoop(statements, address, openType, closeType);
					}
					else
					{
						address++;
					}

				}
			}
			return address;
		}

	}
}
