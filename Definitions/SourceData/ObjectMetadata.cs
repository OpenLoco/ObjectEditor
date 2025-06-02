using OpenLoco.Dat.Data;

namespace OpenLoco.Definitions.SourceData
{
	public record ObjectMetadata(
		string UniqueName,
		string? Description,
		List<string> Authors,
		List<string> Tags,
		List<string> ObjectPacks,
		string? Licence,
		DateTimeOffset? CreationDate,
		DateTimeOffset? LastEditDate,
		DateTimeOffset UploadDate,
		ObjectSource ObjectSource);
}
