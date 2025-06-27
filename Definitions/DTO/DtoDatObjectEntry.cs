namespace OpenLoco.Definitions.DTO
{

	public record DtoDatObjectEntry(
		UniqueObjectId Id,
		string DatName,
		uint DatChecksum,
		ulong xxHash3,
		UniqueObjectId ObjectId)
	{
		public string? DatBytes { get; set; }
	}
}
