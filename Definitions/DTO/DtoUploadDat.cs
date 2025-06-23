namespace OpenLoco.Definitions.DTO
{
	public record DtoUploadDat(
		string DatBytesAsBase64,
		ulong xxHash3,
		DateTimeOffset CreatedDate,
		DateTimeOffset ModifiedDate);
}
