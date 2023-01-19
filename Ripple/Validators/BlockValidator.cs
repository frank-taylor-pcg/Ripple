using Ripple.Exceptions;
using Ripple.Keywords;
using Ripple.Statements;

namespace Ripple.Validators
{
	internal static class BlockValidator
	{
		private static List<string> ValidationErrors = new();

		public static bool Validate(CodeBlock block)
		{
			// Pass 1: Attempt to build each block
			ConstructBlockStatements(block.Statements);

			// Pass 2: Step through the entire code listing and ensure they're all valid, if not log the validation error
			ReportValidationErrors(block.Statements);

			// Pass 3: Verify that all block variables have been defined prior to usage
			CheckVariableDeclarations(block);

			return true;
		}

		private static void ConstructBlockStatements(List<Statement> statements)
		{
			foreach (Statement statement in statements)
			{
				if (statement is IBlockParent blockParent)
				{
					blockParent.ConstructBlock(statements, statement.Address);
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

		private static void CheckVariableDeclarations(CodeBlock block)
		{
			// If the developer has opted to allow implicit variable declarations, then exit without doing anything
			if (!block.UseExplicitVariableDeclarations) return;

			List<string> variableNameDeclarations = new();
			List<string> usedVariables = new();
			List<string> undefinedVariables = new();

			// Gather the list of declared variables and the list of used variables -- do these in the same pass to reduce the number of loops
			foreach (Statement statement in block.Statements)
			{
				if (statement is DeclareVariable declaration)
				{
					variableNameDeclarations.Add(declaration.Name);
				}

				if (statement.Expression is not null && statement.Expression!.Contains("Mem."))
				{
					usedVariables = ExtractVariableNames(statement.Expression);
				}
			}

			// Now cycle through the list of used variable names and catalog the ones that are used, but not declared properly
			foreach (string name in usedVariables)
			{
				if (!variableNameDeclarations.Contains(name))
				{
					undefinedVariables.Add(name);
				}
			}

			if (undefinedVariables.Any())
			{
				string names = string.Join("\n  ", undefinedVariables);
				string msg = $"\nThe following variables are used, but never defined:\n  {names}\n";
				throw new CodeValidationException(msg);
			}
		}

		private static bool IsValidIdentifierCharacter(char c) => (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c == '_');

		//  Return the following as the variable name:
		//		.Mem.Name}"
		private static int FindEndOfIdentifier(string expression, int startIndex)
		{
			int index = startIndex;
			string substring = expression.Substring(startIndex);

			foreach (char c in substring)
			{
				if (!IsValidIdentifierCharacter(c))
				{
					return index;
				}
				index++;
			}

			return -1;
		}

		private static List<string> ExtractVariableNames(string expression)
		{
			List<string> result = new();

			int startIndex = expression.IndexOf(".Mem.") + 5;

			while (startIndex != -1)
			{
				int endIndex = FindEndOfIdentifier(expression, startIndex);

				if (startIndex != endIndex)
				{
					if (endIndex == -1) endIndex = expression.Length;
					result.Add(expression[startIndex..endIndex]);
				}

				startIndex = expression.IndexOf(".Mem.", endIndex);
			}

			return result;
		}

	}
}
