using Definitions;

namespace OpenLoco.Definitions.DTO
{
	public record DtoAuthorEntry(
		DbKey Id,
		string Name) : IHasId;
}
