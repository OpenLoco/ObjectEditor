using OpenLoco.Dat.Data;
using OpenLoco.Dat.Objects;

namespace OpenLoco.Definitions.DTO
{
	public record DtoObjectEntry(
		int Id,
		string InternalName,
		string DisplayName,
		string? Description,
		ObjectSource ObjectSource,
		ObjectType ObjectType,
		VehicleType? VehicleType,
		DateTimeOffset? CreatedDate,
		DateTimeOffset? ModifiedDate,
		DateTimeOffset UploadedDate);
}
