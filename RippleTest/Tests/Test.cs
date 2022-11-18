namespace RippleTest.Tests
{
    internal class Test
    {
        public string Name { get; set; }
        public Action Action { get; set; }
        public Test(string name, Action action)
        {
            Name = name;
            Action = action;
        }
    }
}
