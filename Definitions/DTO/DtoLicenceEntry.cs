using Definitions;

namespace OpenLoco.Definitions.DTO
{
	public record DtoLicenceEntry(
		int Id,
		Guid? GuidId,
		string Name,
		string LicenceText) : IHasId;
}
