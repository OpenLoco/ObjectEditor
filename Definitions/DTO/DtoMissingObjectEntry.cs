using OpenLoco.Dat.Data;

namespace OpenLoco.Definitions.DTO
{
	public record DtoMissingObjectEntry(
		string DatName,
		uint32_t DatChecksum,
		ObjectType ObjectType);
}
