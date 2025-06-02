namespace OpenLoco.Definitions.DTO
{
	public record DtoObjectLookupFromDatEntry(
		int Id,
		string DatName,
		uint DatChecksum,
		ulong xxHash3,
		int ObjectId);
}
