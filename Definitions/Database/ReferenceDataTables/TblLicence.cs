using Definitions.Database.Base;
using Microsoft.EntityFrameworkCore;

namespace Definitions.Database.ReferenceDataTables;

[Index(nameof(Name), IsUnique = true)]
public class TblLicence : DbReferenceObject
{
	public required string Text { get; set; }
}
