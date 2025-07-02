namespace OpenLoco.Definitions.DTO
{
	public record DtoUploadDat(
		string DatBytesAsBase64,
		ulong xxHash3,
		ObjectAvailability InitialAvailability,
		DateTimeOffset CreatedDate,
		DateTimeOffset ModifiedDate);
}
