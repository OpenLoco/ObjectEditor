namespace Definitions.DTO;

/// <summary>A lightweight reference to a scenario file, used as a pack item reference.</summary>
public record DtoScenarioEntry(
	UniqueObjectId Id,
	string Name) : IHasId;

