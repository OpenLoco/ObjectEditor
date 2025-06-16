using Definitions;

namespace OpenLoco.Definitions.DTO
{
	public record DtoAuthorEntry(
		int Id,
		Guid? GuidId,
		string Name) : IHasId;
}
