namespace OpenLoco.Definitions.DTO
{
	public record DtoScenarioEntry(
		UniqueObjectId Id,
		string Name) : IHasId;

	public record DtoScenarioDescriptor(
		UniqueObjectId Id,
		string Name,
		string? Description) : IHasId;
}
