using System.Text;
using Ripple.Exceptions;
using Ripple.Keywords;
using Ripple.Statements;

namespace Ripple.Validators;

internal static class BlockValidator
{
	private static List<string> _validationErrors = new();

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
		_validationErrors = new List<string>();
		foreach (string? msg in
		         from statement in statements
		         where !statement.IsValid()
		         select statement.GetValidationErrorMessage()
		         into msg
		         where msg != null
		         select msg)
		{
			_validationErrors.Add(msg);
		}

		if (_validationErrors.Count > 0)
		{
			throw new CodeValidationException(_validationErrors);
		}
	}

	private static void CheckVariableDeclarations(CodeBlock block)
	{
		// If the developer has opted to allow implicit variable declarations, then exit without doing anything
		if (!block.UseExplicitVariableDeclarations) return;

		List<string> variableNameDeclarations = new();
		List<string> usedVariables = new();

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
		List<string> undefinedVariables = usedVariables
			.Where(name => !variableNameDeclarations.Contains(name))
			.ToList();

		if (!undefinedVariables.Any()) return;
			
		string names = string.Join("\n  ", undefinedVariables);
		StringBuilder sb = new();
		sb.AppendLine("The following variables are used, but never defined:");
		sb.AppendLine($"  {names}");
		// TODO: The usage of explicit/implicit variables should be handled at the VM level, shouldn't it?
		sb.AppendLine("If this was intentional please enable implicit variable usage in the CodeBlock");
		throw new CodeValidationException(sb.ToString());
	}

	private static bool IsValidIdentifierCharacter(char c) => c is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or '_';

	//  Return the following as the variable name:
	//		.Mem.Name}"
	private static int FindEndOfIdentifier(string expression, int startIndex)
	{
		int index = startIndex;
		string substring = expression[startIndex..];

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

		int startIndex = expression.IndexOf(".Mem.", StringComparison.Ordinal);

		while (startIndex != -1)
		{
			// Skip over the .Mem. qualifier
			startIndex += 5;

			// Find the end of the identifier
			int endIndex = FindEndOfIdentifier(expression, startIndex);

			// If we reached the end of the expression, then grab everything to the end
			if (endIndex == -1) endIndex = expression.Length;

			// Add the identifier to the list
			result.Add(expression[startIndex..endIndex]);

			// Calculate the new index
			startIndex = expression.IndexOf(".Mem.", endIndex, StringComparison.Ordinal);
		}

		return result;
	}

}