namespace OpenLoco.Definitions.DTO
{
	public record DtoDatFileDetails(
		string DatName,
		uint DatChecksum,
		ulong xxHash3);
}
