using Ripple.Exceptions;

namespace Ripple.Statements
{
	public class Statement
	{
		public VirtualMachine? VM { get; set; }

		public int Address { get; set; }

		public string? Expression { get; set; }

		public int LineNumber { get; set; }

		public Action? Action { get; set; }

		public Statement(int lineNumber = -1, string? expression = null)
		{
			LineNumber = lineNumber;
			Expression = expression;
		}

		public void Execute(VirtualMachine vm)
		{
			VM = vm;

			try
			{
				if (!IsValid())
				{
					throw new InvalidStatementException(this);
				}

				Action?.Invoke();
			}
			catch (Exception ex)
			{
				throw new RippleException(this, ex);
			}
		}

		// Simple statements should always validate if they compile.  Complex or multi-part statements require a bit more to successfully validate.
		public virtual bool IsValid() => true;

		public virtual void Reset() { }

		public override string ToString()
		{
			// Remove the extraneous bits from the name
			string name = GetType().Name.Split('`')[0];

			return $"[VM/C# {Address,3}/{LineNumber,4}] : {name} {Expression}";
		}

		public virtual string? GetValidationErrorMessage() => $"Error validating {this}";
	}
}
