namespace OpenLoco.Definitions.DTO
{

	public record DtoDatObjectEntry(
		DbKey Id,
		string DatName,
		uint DatChecksum,
		ulong xxHash3,
		DbKey ObjectId)
	{
		public string? DatBytes { get; set; }
	}
}
