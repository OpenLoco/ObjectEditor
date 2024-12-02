using OpenLoco.Dat.Data;

namespace OpenLoco.Definitions.SourceData
{
	public record ObjectMetadata(
		string UniqueName,
		string DatName,
		uint DatChecksum,
		string? Description,
		List<string> Authors,
		List<string> Tags,
		List<string> ObjectPacks,
		string? Licence,
		ObjectAvailability ObjectAvailability,
		DateTimeOffset? CreationDate,
		DateTimeOffset? LastEditDate,
		DateTimeOffset UploadDate,
		ObjectSource ObjectSource);
}
