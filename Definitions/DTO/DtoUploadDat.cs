namespace OpenLoco.Definitions.DTO
{
	public record DtoUploadDat(
		string DatBytesAsBase64,
		DateTimeOffset CreationDate,
		DateTimeOffset ModifiedDate);
}
