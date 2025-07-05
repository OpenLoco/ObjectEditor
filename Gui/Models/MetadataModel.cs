using OpenLoco.Definitions;
using OpenLoco.Definitions.Database;
using OpenLoco.Definitions.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace OpenLoco.Gui.Models
{
	public class MetadataModel(string internalName) : IDbDates
	{
		public string InternalName { get; init; } = internalName;

		public string? Description { get; set; }

		public ObjectAvailability Availability { get; set; }

		public DateOnly? CreatedDate { get; set; }

		public DateOnly? ModifiedDate { get; set; }

		public DateOnly UploadedDate { get; set; }

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
		public IDbSubObject? SubObject { get; set; }
	}
}
