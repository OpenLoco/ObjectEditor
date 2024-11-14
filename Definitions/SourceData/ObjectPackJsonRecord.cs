using OpenLoco.Definitions.Database;
using System.Text.Json.Serialization;

namespace OpenLoco.Definitions.SourceData
{
	[method: JsonConstructor]
	public record ObjectPackJsonRecord(string Name, string? Description, string? Author)
	{
		public ObjectPackJsonRecord(string Name, string? Description, TblAuthor? author) : this(Name, Description, author?.Name)
		{ }
	}
}
