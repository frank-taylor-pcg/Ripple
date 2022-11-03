using Ripple.Exceptions;
using Ripple.Keywords;
using Ripple.Statements;
using Ripple.Validators;
using System.Runtime.CompilerServices;
using System.Text;

namespace Ripple
{
	public class VirtualMachine
	{
		public string Name { get; set; }
		public int TickDelay { get; private set; }
		public int ProgramCounter { get; private set; }
		public bool Verbose { get; set; }
		public object? Result { get; private set; }
		public bool CodeIsValid { get; set; } = false;

		public Func<bool>? Interrupt { get; set; }

		private bool Continue = true;
		private bool ShouldAdvanceProgramCounterThisCycle = true;
		// TODO: Change this back to private readonly when done
		public List<Statement> Statements = new();

		public VirtualMachine(int tickDelay = 10, string name = "Ripple VM")
		{
			TickDelay = tickDelay;
			Name = name;
		}

		public void JumpTo(int address)
		{
			ProgramCounter = address;
			ShouldAdvanceProgramCounterThisCycle = false;
		}

		private void Reset()
		{
			ProgramCounter = 0;
			Continue = true;
		}

		private void AdvanceProgramCounter()
		{
			if (ShouldAdvanceProgramCounterThisCycle)
			{
				ProgramCounter++;
			}
			ShouldAdvanceProgramCounterThisCycle = true;
		}

		private void Validate()
		{
			try
			{
				BlockValidator.Validate(this);
			}
			catch (Exception ex)
			{
				Logger.Log(ex);
			}
		}

		public void Run()
		{
			Validate();

			if (!CodeIsValid)
			{
				System.Diagnostics.Debug.WriteLine($"RippleVM can't run due to invalid code.  See previous debug messages for further details");
				return;
			}

			Reset();

			while (ProgramCounter < Statements.Count && Continue)
			{
				if (HandleInterrupt()) return;
				DoStep();
			}
		}

		private void DoStep()
		{
			try
			{
				if (Verbose)
				{
					Console.WriteLine($"{Statements[ProgramCounter]}");
				}
				Statements[ProgramCounter].Execute();
				AdvanceProgramCounter();
				Thread.Sleep(TickDelay);
			}
			catch (RippleException ex)
			{
				Continue = false;
				System.Diagnostics.Debug.WriteLine(ex);
			}
		}

		private bool HandleInterrupt()
		{
			if (Interrupt is null || !Interrupt()) return false;

			StringBuilder sb = new();

			sb.AppendLine($"\nProcess interrupted. Current VM state:");
			sb.AppendLine($"  Halted during execution of {Statements[ProgramCounter - 1]}");

			Logger.Log(sb.ToString());

			return true;
		}

		public string ProgramAsString()
		{
			StringBuilder sb = new();

			foreach (Statement statement in Statements)
			{
				sb.AppendLine(statement.ToString());
			}

			return sb.ToString();
		}

		private void Add(Statement statement)
		{
			Statements.Add(statement);
			statement.Address = Statements.Count - 1;
		}

		#region API
		public VirtualMachine If(Func<bool> condition, [CallerLineNumber] int lineNumber = -1, [CallerArgumentExpression("condition")] string? expression = null)
		{
			Add(new If(this, condition, lineNumber, expression));
			return this;
		}
		public VirtualMachine ElseIf(Func<bool> condition, [CallerLineNumber] int lineNumber = -1, [CallerArgumentExpression("condition")] string? expression = null)
		{
			Add(new ElseIf(this, condition, lineNumber, expression));
			return this;
		}
		public VirtualMachine Else([CallerLineNumber] int lineNumber = -1)
		{
			Add(new Else(this, lineNumber));
			return this;
		}
		public VirtualMachine EndIf([CallerLineNumber] int lineNumber = -1)
		{
			Add(new EndIf(this, lineNumber));
			return this;
		}

		public VirtualMachine Switch(Func<object> valueLambda, [CallerLineNumber] int lineNumber = -1, [CallerArgumentExpression("valueLambda")] string? expression = null)
		{
			Add(new Switch(this, valueLambda, lineNumber, expression));
			return this;
		}
		public VirtualMachine Case(object value, [CallerLineNumber] int lineNumber = -1, [CallerArgumentExpression("value")] string? expression = null)
		{
			Add(new Case(this, value, lineNumber, expression));
			return this;
		}
		public VirtualMachine Break([CallerLineNumber] int lineNumber = -1)
		{
			Add(new Break(this, lineNumber));
			return this;
		}
		public VirtualMachine Default([CallerLineNumber] int lineNumber = -1)
		{
			Add(new Default(this, lineNumber));
			return this;
		}
		public VirtualMachine EndSwitch([CallerLineNumber] int lineNumber = -1)
		{
			Add(new EndSwitch(this, lineNumber));
			return this;
		}

		public VirtualMachine While(Func<bool> condition, [CallerLineNumber] int lineNumber = -1, [CallerArgumentExpression("condition")] string? expression = null)
		{
			Add(new While(this, condition, lineNumber, expression));
			return this;
		}
		public VirtualMachine EndWhile([CallerLineNumber] int lineNumber = -1)
		{
			Add(new EndWhile(this, lineNumber));
			return this;
		}

		public VirtualMachine Repeat([CallerLineNumber] int lineNumber = -1)
		{
			Add(new Repeat(this, lineNumber));
			return this;
		}
		public VirtualMachine Until(Func<bool> condition, [CallerLineNumber] int lineNumber = -1, [CallerArgumentExpression("condition")] string? expression = null)
		{
			Add(new Until(this, condition, lineNumber, expression));
			return this;
		}

		public VirtualMachine CSAction(Action act, [CallerLineNumber] int lineNumber = -1, [CallerArgumentExpression("act")] string? expression = null)
		{
			Add(new CSAction(this, act, lineNumber, expression));
			return this;
		}

		public VirtualMachine CSFunc<T>(Func<T> func, [CallerLineNumber] int lineNumber = -1, [CallerArgumentExpression("func")] string? expression = null)
		{
			Action act = () =>
			{
				if (func is not null)
				{
					T? t = func!();
					Result = t!;
				}
			};
			Add(new CSAction(this, act, lineNumber, expression));
			return this;
		}

		// This is going to get tricky -- how do I properly track VM variables and their scopes?
		public VirtualMachine For(
			Func<bool> checkLambda,
			Action iteratorLambda,
			[CallerLineNumber] int lineNumber = -1,
			[CallerArgumentExpression("checkLambda")] string? checkString = null,
			[CallerArgumentExpression("iteratorLambda")] string? iteratorString = null
			)
		{
			string expression = $"{checkString} ; {iteratorString}";
			Add(new For(this, checkLambda, iteratorLambda, lineNumber, expression));
			return this;
		}
		public VirtualMachine EndFor([CallerLineNumber] int lineNumber = -1)
		{
			Add(new EndFor(this, lineNumber));
			return this;
		}

		// TODO: Determine if functions are even necessary
		// TODO: There should be a more elegant way to accomplish function definitions and calls than this.
		public VirtualMachine Def(string functionName, List<Argument> argList, [CallerLineNumber] int lineNumber = -1)
		{
			string arguments = string.Join(", ", argList.Select(x => $"{x.Type.Name} {x.Name}").ToList());
			string expression = $"{functionName}({arguments})";
			Add(new Def(this, functionName, argList, lineNumber, expression));
			return this;
		}
		public VirtualMachine EndDef([CallerLineNumber] int lineNumber = -1)
		{
			Add(new EndDef(this, lineNumber));
			return this;
		}
		public VirtualMachine Call(string functionName, List<Func<object>> parameterLambdas, [CallerLineNumber] int lineNumber = -1)
		{
			string arguments = string.Join(", ", parameterLambdas);
			string expression = $"{functionName}({arguments})";
			Add(new Call(this, functionName, parameterLambdas, lineNumber, expression));
			return this;
		}


		#endregion
	}
}
