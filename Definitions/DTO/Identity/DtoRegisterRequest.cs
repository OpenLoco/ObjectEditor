namespace OpenLoco.Definitions.DTO.Identity
{
	public record DtoRegisterRequest(string Username, string Password);
	public record DtoCreateRoleRequest(string RoleName);
}
