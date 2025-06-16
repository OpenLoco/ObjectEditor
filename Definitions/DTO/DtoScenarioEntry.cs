using Definitions;

namespace OpenLoco.Definitions.DTO
{
	public record DtoScenarioEntry(
		int Id,
		Guid? GuidId,
		string Name) : IHasId;

	public record DtoScenarioDescriptor(
		int Id,
		Guid? GuidId,
		string Name,
		string? Description) : IHasId;
}
