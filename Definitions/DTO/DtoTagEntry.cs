namespace Definitions.DTO;

public record DtoTagEntry(
	UniqueObjectId Id,
	string Name) : IHasId;
