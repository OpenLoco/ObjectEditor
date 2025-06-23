using Definitions;

namespace OpenLoco.Definitions.DTO
{
	public record DtoTagEntry(
		DbKey Id,
		string Name) : IHasId;
}
