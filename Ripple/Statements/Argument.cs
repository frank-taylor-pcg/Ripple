namespace Ripple.Statements;

public abstract class Argument
{
	public string Name { get; set; }
	public Type Type { get; set; }

	protected Argument(string name, Type type)
	{
		Name = name ?? throw new ArgumentNullException(nameof(name));
		Type = type ?? throw new ArgumentNullException(nameof(type));
	}
}