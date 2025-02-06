using Definitions;
using Definitions.Database.Objects;
using OpenLoco.Dat.Data;
using OpenLoco.Dat.Objects;
using OpenLoco.Definitions.Database;

namespace OpenLoco.Definitions.DTO
{
	public record DtoObjectDescriptorWithMetadata(
		int Id,
		string UniqueName,
		string DatName,
		uint DatChecksum,
		string? DatBytes, // base64-encoded
		ObjectSource ObjectSource,
		ObjectType ObjectType,
		VehicleType? VehicleType,
		string? Description,
		ICollection<TblAuthor> Authors,
		DateTimeOffset? CreationDate,
		DateTimeOffset? LastEditDate,
		DateTimeOffset UploadDate,
		ICollection<TblTag> Tags,
		ICollection<TblLocoObjectPack> ObjectPacks,
		ObjectAvailability Availability,
		TblLicence? Licence) : IHasId;
}
