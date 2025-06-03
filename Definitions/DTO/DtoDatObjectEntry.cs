namespace OpenLoco.Definitions.DTO
{

	public record DtoDatObjectEntry(
		int Id,
		string DatName,
		uint DatChecksum,
		ulong xxHash3,
		int ObjectId)
	{
		public string? DatBytes { get; set; }
	}
}
