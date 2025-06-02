using Microsoft.EntityFrameworkCore;

namespace OpenLoco.Definitions.Database
{
	[Index(nameof(DatName), nameof(DatChecksum), IsDescending = [true, false], IsUnique = true)]
	[Index(nameof(xxHash3), IsUnique = true)]
	public class TblObjectLookupFromDat
	{
		public int Id { get; set; }

		public required string DatName { get; set; }

		public required uint DatChecksum { get; set; }

		public required ulong xxHash3 { get; set; } // technically a byte[], but we'll use 64-bit int for faster comparison

		public required int ObjectId { get; set; } // navigation property
		public required TblLocoObject Object { get; set; } // fk property
	}
}
