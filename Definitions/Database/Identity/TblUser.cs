using Definitions.Database.ReferenceDataTables;
using Microsoft.AspNetCore.Identity;

namespace Definitions.Database.Identity;

public class TblUser : IdentityUser<UniqueObjectId>, IHasId
{
	public UniqueObjectId? AssociatedAuthorId { get; set; }
	public TblAuthor? AssociatedAuthor { get; set; }
}
