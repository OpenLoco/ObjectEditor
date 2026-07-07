namespace Definitions.ObjectModels.Objects.TownNames;

public readonly struct StringTableEntry
{
	public string Text { get; }

	public LocationFlags LocationHint { get; }

	public StringTableEntry(string text, LocationFlags type)
	{
		Text = text;
		LocationHint = type;
	}
}
