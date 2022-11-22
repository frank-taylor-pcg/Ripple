using Ripple.Statements;

namespace Ripple.Validators
{
    public static class LoopConstructor
    {
        public static int ConstructLoop(List<Statement> statements, int startAddress, Type openType, Type closeType)
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
                        address = ConstructLoop(statements, address, openType, closeType);
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
