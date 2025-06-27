using Definitions;
using OpenLoco.Definitions.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace OpenLoco.Gui.Models
{
	public class MetadataModel(string internalName)
	{
		public string InternalName { get; init; } = internalName;

		public string? Description { get; set; }

		public ObjectAvailability Availability { get; set; }

		public DateTimeOffset? CreatedDate { get; set; }

		public DateTimeOffset? ModifiedDate { get; set; }

		public DateTimeOffset UploadedDate { get; set; }

		public TblLicence? Licence { get; set; }

		[Browsable(false)]
		public ICollection<TblAuthor> Authors { get; set; } = [];

		[Browsable(false)]
		public ICollection<TblTag> Tags { get; set; } = [];

		[Browsable(false)]
		public ICollection<TblObjectPack> ObjectPacks { get; set; } = [];

		[Browsable(false)]
		public ICollection<TblDatObject> DatObjects { get; set; } = [];
	}
}
