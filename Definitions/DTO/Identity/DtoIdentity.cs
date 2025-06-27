using Definitions;

namespace OpenLoco.Definitions.DTO.Identity
{
	public record DtoRegisterRequest(string Email, string UserName, string Password);
	public record DtoLoginRequest(string Email, string Password);

	public record DtoInfoResponse(string UserName, string Email, bool EmailIsConfirmed);

	public record DtoRoleEntry(UniqueObjectId Id, string Name) : IHasId;
	//public record DtoRoleDescriptor(UniqueObjectId Id, string Name) : DtoWithDbKey(Id), IHasId { }

	public record DtoUserEntry(UniqueObjectId Id, string UserName) : IHasId;
	//public record DtoUserDescriptor(UniqueObjectId Id, string Username, string Password) : DtoWithDbKey(Id), IHasId { }
}
