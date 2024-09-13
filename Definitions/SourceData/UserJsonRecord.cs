namespace OpenLoco.Definitions.SourceData
{
	public record UserJsonRecord(string Name, string DisplayName, string? Author, List<string> Roles, string PasswordHashed, string PasswordSalt);
}
