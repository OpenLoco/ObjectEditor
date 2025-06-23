using Definitions;

namespace OpenLoco.Definitions.DTO
{
	public record DtoScenarioEntry(
		DbKey Id,
		string Name) : IHasId;

	public record DtoScenarioDescriptor(
		DbKey Id,
		string Name,
		string? Description) : IHasId;
}
