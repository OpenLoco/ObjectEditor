using Dat.Data;

namespace Definitions.DTO
{
	public record DtoMissingObjectEntry(
		string DatName,
		uint32_t DatChecksum,
		ObjectType ObjectType);
}
