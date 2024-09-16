using OpenLoco.Definitions.Database;
using System.Text.Json.Serialization;

namespace OpenLoco.Definitions.SourceData
{
	[method: JsonConstructor]
	public record ModpackJsonRecord(string Name, string? Author)
	{
		public ModpackJsonRecord(string Name, TblAuthor? author) : this(Name, author?.Name)
		{ }
	}
}
