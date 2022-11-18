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
		public object? Result { get; internal set; }
		public bool CodeIsValid { get; set; } = false;

		public Func<bool>? Interrupt { get; set; }

		public CodeBlock? CodeBlock { get; set; }

		private bool Continue = true;
		private bool ShouldAdvanceProgramCounterThisCycle = true;

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

		public void Run()
		{
			// TODO: Report to the user that a program needs to be loaded into the VM
			if (CodeBlock is null) return;

			CodeBlock.Validate();

			//if (!CodeIsValid)
			//{
			//	System.Diagnostics.Debug.WriteLine($"Ripple can't run due to invalid code.  See previous debug messages for further details");
			//	return;
			//}

			Reset();

			while (ProgramCounter < CodeBlock.Statements.Count && Continue)
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
					Console.WriteLine($"{CodeBlock!.Statements[ProgramCounter]}");
				}
				CodeBlock!.Statements[ProgramCounter].Execute(this);
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
			sb.AppendLine($"  Halted during execution of {CodeBlock!.Statements[ProgramCounter - 1]}");

			Logger.Log(sb.ToString());

			return true;
		}

		public string ProgramAsString()
		{
			if (CodeBlock is null) return string.Empty;

			StringBuilder sb = new();

			foreach (Statement statement in CodeBlock!.Statements)
			{
				sb.AppendLine(statement.ToString());
			}

			return sb.ToString();
		}
	}
}
