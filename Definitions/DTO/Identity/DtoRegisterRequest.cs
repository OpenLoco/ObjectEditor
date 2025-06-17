using Definitions;

namespace OpenLoco.Definitions.DTO.Identity
{
	public abstract record DtoWithDbKey(DbKey Id) : IHasId;

	public record DtoRoleEntry(DbKey Id, string Name) : IHasId;
	//public record DtoRoleDescriptor(DbKey Id, string Name) : DtoWithDbKey(Id), IHasId { }

	public record DtoUserEntry(DbKey Id, string UserName) : IHasId;
	//public record DtoUserDescriptor(DbKey Id, string Username, string Password) : DtoWithDbKey(Id), IHasId { }
}
