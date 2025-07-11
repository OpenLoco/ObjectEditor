using Definitions.Database;

namespace Definitions.DTO;

public record DtoItemPackEntry(
	UniqueObjectId Id,
	string Name,
	string? Description,
	DateOnly? CreatedDate,
	DateOnly? ModifiedDate,
	DateOnly UploadedDate,
	DtoLicenceEntry? Licence) : IHasId, IDbDates;
