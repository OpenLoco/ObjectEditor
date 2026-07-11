using System.Text.Json.Serialization;

namespace Definitions.ObjectModels.Objects.TownNames;

public readonly struct StringTableEntry
{
	public string Text { get; }

	public LocationFlags LocationHint { get; }

	[JsonConstructor]
	public StringTableEntry(string text, LocationFlags locationHint)
	{
		Text = text;
		LocationHint = locationHint;
	}
}