namespace Ripple.Statements
{
	public class Argument
	{
		public string Name { get; set; }
		public Type Type { get; set; }

		public Argument(string name, Type type)
		{
			Name = name;
			Type = type;
		}
	}
}
