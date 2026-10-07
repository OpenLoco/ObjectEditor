using Microsoft.EntityFrameworkCore;

namespace Definitions.Database;

// A binary-different file can carry the same S5 name and checksum (e.g. a re-packed/hacked object),
// so the (DatName, DatChecksum) pair is an index, not a unique key. xxHash3 over the whole file is the
// authoritative identity: two files with the same hash are the same file and only the oldest is kept.
[Index(nameof(DatName), nameof(DatChecksum), IsDescending = [true, false])]
[Index(nameof(xxHash3), IsUnique = true)]
public class TblDatObject : DbIdObject
{
	public required string DatName { get; set; }

	public required uint DatChecksum { get; set; }

	public required ulong xxHash3 { get; set; } // technically a byte[], but we'll use 64-bit int for faster comparison

	public required UniqueObjectId ObjectId { get; set; } // FK property

	public TblObject Object { get; set; } = null!; // navigation property. a DAT object must ALWAYS reference an OL object
}
