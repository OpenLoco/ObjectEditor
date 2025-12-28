using Definitions.ObjectModels.Types;

namespace Definitions.DTO;

public record DtoObjectMissingEntry(
	string DatName,
	uint32_t DatChecksum,
	ObjectType ObjectType);
