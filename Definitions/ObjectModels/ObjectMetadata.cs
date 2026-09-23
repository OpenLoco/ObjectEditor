using Definitions.Database;
using Definitions.DTO;
using Definitions.ObjectModels.Objects.Vehicle;
using System.ComponentModel;

namespace Definitions.ObjectModels;

public class ObjectMetadata(string internalName)
{
	public UniqueObjectId UniqueObjectId { get; init; }

	public string InternalName { get; init; } = internalName;

	public string? Description { get; set; }

	public ObjectAvailability Availability { get; set; }

	/// <summary>
	/// The vehicle type of a vehicle object, as stored on the server. Carried so that a metadata upload
	/// round-trips it instead of clearing it (the server applies the whole <c>Objects</c> row).
	/// </summary>
	public VehicleType? VehicleType { get; set; }

	public DateTimeOffset? CreatedDate { get; set; }

	public DateTimeOffset? ModifiedDate { get; set; }

	public DateTimeOffset UploadedDate { get; set; }

	public DtoLicenceEntry? Licence { get; set; }

	[Browsable(false)]
	public ICollection<DtoAuthorEntry> Authors { get; set; } = [];

	[Browsable(false)]
	public ICollection<DtoTagEntry> Tags { get; set; } = [];

	[Browsable(false)]
	public ICollection<DtoItemPackEntry> ObjectPacks { get; set; } = [];

	[Browsable(false)]
	public ICollection<DtoDatObjectEntry> DatObjects { get; set; } = [];

	[Browsable(false)]
	public IDtoSubObject? SubObject { get; set; }
}
