namespace OpenLoco.Definitions.DTO.Identity
{
	public abstract record DtoWithDbKey(DbKey Id);

	public record DtoRoleCreate(string Name);
	public record DtoRoleModify(DbKey Id, string Name) : DtoWithDbKey(Id) { }

	public record DtoUserCreate(string Username, string Password);
	public record DtoUserModify(DbKey Id, string Username, string Password) : DtoWithDbKey(Id) { }
}
