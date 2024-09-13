namespace OpenLoco.Definitions.DTO
{
	public enum DataOperation
	{
		Create,
		Read,
		Update,
		Delete,
	}

	public record DtoCreateUser(
		string Name,
		string DisplayName,
		string? Author,
		string Password);

	public record DtoChangeUserRole(
		string UserName,
		string RoleName,
		DataOperation Op);

	public record DtoChangeUserAuthor(
		string UserName,
		string AuthorName,
		DataOperation Op);
}
