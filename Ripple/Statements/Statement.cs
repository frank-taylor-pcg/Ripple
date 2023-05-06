using Ripple.Exceptions;

namespace Ripple.Statements;

public abstract class Statement
{
	public VirtualMachine? Vm { get; set; }

	public int Address { get; set; }

	public string? Expression { get; set; }

	public int LineNumber { get; set; }

	public Action? Action { get; set; }

	protected Statement(int lineNumber = -1, string? expression = null)
	{
		LineNumber = lineNumber;
		Expression = expression;
	}

	public void Execute(VirtualMachine vm)
	{
		Vm = vm;

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

	public string GetName()
	{
		// Remove the extraneous bits from the name
		return GetType().Name.Split('`')[0];
	}

	public override string ToString()
	{
		return $"[VM/C# {Address,3}/{LineNumber,4}] : {GetName()} {Expression}";
	}

	public virtual string? GetValidationErrorMessage() => $"Error validating {this}";
}