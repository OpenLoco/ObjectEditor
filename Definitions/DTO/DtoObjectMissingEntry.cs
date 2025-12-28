using Definitions.ObjectModels.Types;

namespace Definitions.DTO;

public record DtoObjectMissingEntry(
	UniqueObjectId Id,
	string DatName,
	uint32_t DatChecksum,
	ObjectType ObjectType) : IHasId;
