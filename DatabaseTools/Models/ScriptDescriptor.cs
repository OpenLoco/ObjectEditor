namespace DatabaseTools.Models;

// Represents a runnable script that can be invoked from the UI. Each tab shows a list of these.
public sealed class ScriptDescriptor
{
	public string Name { get; }
	public string? Description { get; }
	public Func<ToolsSettings, Action<string>, Task> Run { get; }

	public ScriptDescriptor(string name, string? description, Func<ToolsSettings, Action<string>, Task> run)
	{
		Name = name;
		Description = description;
		Run = run;
	}
}
