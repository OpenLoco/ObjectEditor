using OpenLoco.Dat.Data;
using OpenLoco.Dat.Objects;

namespace OpenLoco.Definitions.DTO
{
	public record DtoObjectIndexEntry(
		int Id,
		string DatName,
		uint DatChecksum,
		ObjectType ObjectType,
		ObjectSource ObjectSource,
		VehicleType? VehicleType);
}
