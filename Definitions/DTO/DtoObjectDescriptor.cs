using Dat.Data;
using Dat.Objects;
using Definitions.Database;

namespace Definitions.DTO;

public record DtoObjectDescriptor(
	UniqueObjectId Id,
	string Name,
	string DisplayName,
	uint? DatChecksum,
	string? Description,
	ObjectSource ObjectSource,
	ObjectType ObjectType,
	VehicleType? VehicleType,
	ObjectAvailability Availability,
	DateOnly? CreatedDate,
	DateOnly? ModifiedDate,
	DateOnly UploadedDate,
	DtoLicenceEntry? Licence,
	ICollection<DtoAuthorEntry> Authors,
	ICollection<DtoTagEntry> Tags,
	ICollection<DtoItemPackEntry> ObjectPacks,
	ICollection<DtoDatObjectEntry> DatObjects,
	DtoStringTableDescriptor StringTable
	//IDtoSubObject SubObject
	) : IHasId, IDbDates;
