using Definitions.ObjectModels.Types;

namespace Definitions.DTO;

public record DtoObjectMissingPost(
	string DatName,
	uint32_t DatChecksum,
	ObjectType ObjectType);
