using Dat.Data;
using Dat.Objects;
using Definitions.Database;

namespace Definitions.DTO;

public record DtoObjectEntry(
	UniqueObjectId Id,
	string InternalName,
	string DisplayName,
	uint32_t? DatChecksum,
	string? Description,
	ObjectSource ObjectSource,
	ObjectType ObjectType,
	VehicleType? VehicleType,
	ObjectAvailability Availability,
	DateOnly? CreatedDate,
	DateOnly? ModifiedDate,
	DateOnly UploadedDate) : IHasId, IDbDates;
