using Ripple.Exceptions;
using Ripple.Statements;

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
