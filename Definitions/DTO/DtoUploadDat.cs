namespace Definitions.DTO
{
	public record DtoUploadDat(
		string DatBytesAsBase64,
		ulong xxHash3,
		ObjectAvailability InitialAvailability,
		DateOnly CreatedDate,
		DateOnly ModifiedDate);
}
