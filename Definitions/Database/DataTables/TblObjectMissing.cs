using Definitions.ObjectModels.Types;
using Microsoft.EntityFrameworkCore;

namespace Definitions.Database;

[Index(nameof(DatName), nameof(DatChecksum), IsUnique = true)]
public class TblObjectMissing : DbIdObject
{
	public required string DatName { get; set; }

	public required uint DatChecksum { get; set; }

	public required ObjectType ObjectType { get; set; }
}
