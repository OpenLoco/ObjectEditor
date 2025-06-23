using Definitions;

namespace OpenLoco.Definitions.DTO
{
	public record DtoLicenceEntry(
		DbKey Id,
		string Name,
		string LicenceText) : IHasId;
}
