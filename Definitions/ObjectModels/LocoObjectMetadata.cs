using Definitions.Database;
using Definitions.DTO;
using System.ComponentModel;

namespace Definitions.ObjectModels;

public class LocoObjectMetadata(string internalName)
{
	public UniqueObjectId UniqueObjectId { get; init; }

	public string InternalName { get; init; } = internalName;

	public string? Description { get; set; }

	public ObjectAvailability Availability { get; set; }

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
