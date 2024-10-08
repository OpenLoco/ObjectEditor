using OpenLoco.Dat.Data;
using OpenLoco.Dat.Objects;

namespace OpenLoco.Definitions.DTO
{
	public record DtoObjectIndexEntry(
		int Id,
		string DatName,
		uint DatChecksum,
		ObjectType ObjectType,
		bool IsVanilla,
		VehicleType? VehicleType);
}
