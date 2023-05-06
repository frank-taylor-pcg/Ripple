using System.Reflection.Metadata;
using Ripple.Exceptions;
using System.Text;

namespace Ripple;

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

  private bool _continue = true;
  private bool _shouldAdvanceProgramCounterThisCycle = true;
  private bool _shouldDumpState = true;

  public VirtualMachine(int tickDelay = 10, string name = "Ripple VM")
  {
    TickDelay = tickDelay;
    Name = name;
  }

  public void JumpTo(int address)
  {
    ProgramCounter = address;
    _shouldAdvanceProgramCounterThisCycle = false;
  }

  public void Reset()
  {
    ProgramCounter = 0;
    _continue = true;
  }

  private void AdvanceProgramCounter()
  {
    if (_shouldAdvanceProgramCounterThisCycle)
    {
      ProgramCounter++;
    }

    _shouldAdvanceProgramCounterThisCycle = true;
  }

  public void Run()
  {
    // TODO: Track the duration of each step and give alerts for slow running statements.
    // TODO: Report to the user that a program needs to be loaded into the VM
    if (CodeBlock is null) return;

    CodeBlock.Validate();

    //if (!CodeIsValid)
    //{
    //	System.Diagnostics.Debug.WriteLine($"Ripple can't run due to invalid code.  See previous debug messages for further details");
    //	return;
    //}

    Reset();

    while (ProgramCounter < CodeBlock.Statements.Count && _continue)
    {
      // Instead of exiting, should pause and allow for resuming
      // Else : VM just cycles without doing anything if interrupted. This is intentional.

      if (HandleInterrupt()) continue;
      _shouldDumpState = true;
      DoStep();
    }
  }
  
  // Temporary until I think about the proper way to do this
  public void Step()
  {
    if (CodeBlock is null) return;
    if (ProgramCounter >= CodeBlock.Statements.Count) return;
    if (HandleInterrupt()) return;
    
    if (ProgramCounter == 0)
    {
      CodeBlock.Validate();
      Reset();
      _shouldDumpState = true;
    }

    DoStep();
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
      _continue = false;
      System.Diagnostics.Debug.WriteLine(ex);
    }
  }

  private bool HandleInterrupt()
  {
    if (Interrupt is null || !Interrupt()) return false;

    if (!_shouldDumpState) return true;

    _shouldDumpState = false;
    DumpState();
    return true;
  }

  private void DumpState()
  {
    StringBuilder sb = new();

    sb.AppendLine("\nProcess interrupted. Current VM state:");
    sb.AppendLine($"  Halted during execution of {CodeBlock!.Statements[ProgramCounter]}");
    if (CodeBlock is not null)
      sb.AppendLine(CodeBlock.ToString());

    Logger.Log(sb.ToString());
  }
}