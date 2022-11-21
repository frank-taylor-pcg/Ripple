using Ripple;

namespace RippleTest
{
    internal class Test
    {
        public string Name { get; set; }
        public Action<VirtualMachine> Action { get; set; }
        public Test(string name, Action<VirtualMachine> action)
        {
            Name = name;
            Action = action;
        }
    }
}
