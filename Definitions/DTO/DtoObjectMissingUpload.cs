using Definitions.ObjectModels.Types;

namespace Definitions.DTO;

public record DtoObjectMissingUpload(
	string DatName,
	uint32_t DatChecksum,
	ObjectType ObjectType);
