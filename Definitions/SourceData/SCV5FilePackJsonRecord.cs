using OpenLoco.Definitions.Database;
using System.Text.Json.Serialization;

namespace OpenLoco.Definitions.SourceData
{
	[method: JsonConstructor]
	public record SCV5FilePackJsonRecord(string Name, string? Description, string? Author)
	{
		public SCV5FilePackJsonRecord(string Name, string? Description, TblAuthor? author) : this(Name, Description, author?.Name)
		{ }
	}
}
