using Microsoft.EntityFrameworkCore;

namespace OpenLoco.Definitions.Database
{
	[Index(nameof(DatName), nameof(DatChecksum), IsDescending = [true, false], IsUnique = true)]
	[Index(nameof(xxHash3), IsUnique = true)]
	public class TblDatObject : DbIdObject
	{
		public DbKey Id { get; set; }

		public required string DatName { get; set; }

		public required uint DatChecksum { get; set; }

		public required ulong xxHash3 { get; set; } // technically a byte[], but we'll use 64-bit int for faster comparison

		public required DbKey ObjectId { get; set; } // FK property

		public TblObject Object { get; set; } // navigation property. a DAT object must ALWAYS reference an OL object
	}
}
