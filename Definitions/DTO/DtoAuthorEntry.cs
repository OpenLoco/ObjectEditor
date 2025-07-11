namespace Definitions.DTO;

public record DtoAuthorEntry(
	UniqueObjectId Id,
	string Name) : IHasId;
