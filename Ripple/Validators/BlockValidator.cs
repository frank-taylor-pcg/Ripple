using Ripple.Exceptions;
using Ripple.Keywords;
using Ripple.Statements;
using System.Xml.Linq;

namespace Ripple.Validators
{
	internal static class BlockValidator
	{
		private static List<string> ValidationErrors = new();

		public static bool Validate(List<Statement> statements)
		{
			// Pass 1: Attempt to build each block
			ConstructBlockStatements(statements);

			// Pass 2: Step through the entire code listing and ensure they're all valid, if not log the validation error
			ReportValidationErrors(statements);

			// Pass 3: Verify that all block variables have been defined prior to usage
			CheckVariableDeclarations(statements);

			return true;
		}

		private static void ConstructBlockStatements(List<Statement> statements)
		{
			foreach (Statement statement in statements)
			{
				if (statement is If @if)
				{
					ConstructIfBlock(statements, @if.Address);
				}

				if (statement is Switch @switch)
				{
					ConstructSwitchBlock(statements, @switch.Address);
				}

				if (statement is While @while)
				{
					ConstructGenericLoop(statements, @while.Address, typeof(While), typeof(EndWhile));
				}

				if (statement is Repeat @repeat)
				{
					ConstructGenericLoop(statements, @repeat.Address, typeof(Repeat), typeof(Until));
				}

				if (statement is For @for)
				{
					ConstructGenericLoop(statements, @for.Address, typeof(For), typeof(EndFor));
				}
			}
		}

		private static void ReportValidationErrors(List<Statement> statements)
		{
			ValidationErrors = new();
			foreach (Statement statement in statements)
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

		private static void CheckVariableDeclarations(List<Statement> statements)
		{
			//List<string> variableNameDeclarations = new();
			//List<string> undefinedVariables = new();

			//foreach (Statement statement in statements)
			//{
			//	if (statement is DeclareVariable declaration)
			//	{
			//		variableNameDeclarations.Add(declaration.Name);
			//	}

			//	if (statement.Expression!.Contains("Mem."))
			//	{
			//		List<string> variableNames = ExtractVariableNames(statement.Expression);
			//		// Need to grab the list of variableNames that aren't in our list of declarations

			//		if (!variableNameDeclarations.Contains(variableName))
			//		{
			//			undefinedVariables.Add($"({statement.LineNumber}) Variable '{variableName}' is undefined");
			//		}
			//	}
			//}

			//if (undefinedVariables.Any())
			//{
			//	throw new CodeValidationException(undefinedVariables);
			//}
		}

		private static List<string> ExtractVariableNames(string expression)
		{
			List<string> result = new();

			int startIndex = expression.IndexOf(".Mem.");

			while (startIndex != -1)
			{
				int endIndex = expression.IndexOf(' ', startIndex);

				if (startIndex != endIndex)
				{
					if (endIndex == -1) endIndex = expression.Length;
					result.Add(expression.Substring(startIndex, endIndex - startIndex));
				}

				startIndex = expression.IndexOf(".Mem.", endIndex);
			}

			return result;
		}

	}
}
