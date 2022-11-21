using Ripple;

namespace RippleTest
{
	internal static class TestFactory
	{
		internal static void TestIfBlock(VirtualMachine vm)
		{
			// Running this multiple times to showcase that all conditional checks work.  The side-effect of this is that interrupting it only interrupts one iteration.
			CodeBlock cb = CodeBlockFactory.CreateIfBlock();
			List<string> fruits = new() { "apple", "orange", "banana", "stapler" };
			vm.CodeBlock = cb;
			foreach (string fruit in fruits)
			{
				vm.CodeBlock.Mem.Fruit = fruit;
				vm.Run();
			}
		}

		internal static void TestSwitchBlock(VirtualMachine vm)
		{
			vm.CodeBlock = CodeBlockFactory.CreateSwitchBlock();
			for (int i = 0; i < 10; i++)
			{
				vm.CodeBlock.Mem.Choice = i;
				vm.Run();
			}
		}

		internal static void TestWhileBlock(VirtualMachine vm)
		{
			vm.CodeBlock = CodeBlockFactory.CreateWhileBlock();
			vm.Run();
		}

		internal static void TestRepeatBlock(VirtualMachine vm)
		{
			vm.CodeBlock = CodeBlockFactory.CreateRepeatBlock();
			vm.Run();
		}

		internal static void TestForLoop(VirtualMachine vm)
		{
			vm.CodeBlock = CodeBlockFactory.CreateForBlock();
			vm.Run();
		}

		internal static void TestInterrupt(VirtualMachine vm)
		{
			vm.CodeBlock = CodeBlockFactory.CreateLongRunningProcess();
			vm.Run();
		}
	}
}
