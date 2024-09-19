namespace OpenLoco.Definitions.SourceData
{
	public record ObjectMetadata(string DatName, uint Checksum, string? Description, List<string> Authors, List<string> Tags, List<string> Modpacks, string? Licence);
}
