namespace Definitions.DTO;

public record DtoObjectPost(
	string DatBytesAsBase64,
	ulong xxHash3,
	ObjectAvailability InitialAvailability,
	DateOnly CreatedDate,
	DateOnly ModifiedDate);
