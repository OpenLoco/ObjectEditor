using Definitions;

namespace OpenLoco.Definitions.DTO
{
	public record DtoTagEntry(
		int Id,
		Guid? GuidId,
		string Name) : IHasId;
}
