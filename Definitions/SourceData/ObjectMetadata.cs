namespace OpenLoco.Definitions.SourceData
{
	public record ObjectMetadata(string UniqueName, string ObjectName, uint ObjectChecksum, string? Description, List<string> Authors, List<string> Tags, List<string> Modpacks, string? Licence);
}
