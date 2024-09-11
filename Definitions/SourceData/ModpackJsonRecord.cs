using OpenLoco.Definitions.Database;

namespace OpenLoco.Definitions.SourceData
{
	public record ModpackJsonRecord(string Name, string? Author)
	{
		public ModpackJsonRecord(string Name, TblAuthor? author) : this(Name, author?.Name)
		{ }
	}
}
