namespace Definitions.DTO
{
	public record DtoLicenceEntry(
		UniqueObjectId Id,
		string Name,
		string LicenceText) : IHasId;
}
