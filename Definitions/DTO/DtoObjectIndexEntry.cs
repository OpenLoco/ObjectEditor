using OpenLoco.Dat.Data;
using OpenLoco.Dat.Objects;

namespace OpenLoco.Definitions.DTO
{
	public record DtoObjectIndexEntry(
		int Id,
		string DatName,
		uint DatChecksum,
		ObjectSource ObjectSource,
		ObjectType ObjectType,
		VehicleType? VehicleType,
		ObjectAvailability ObjectAvailability,
		string InternalName,
		string? Description,
		DateTimeOffset? CreationDate,
		DateTimeOffset? LastEditDate,
		DateTimeOffset UploadDate);
}
