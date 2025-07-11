using Microsoft.AspNetCore.Identity;

namespace Definitions.Database;

public class TblUser : IdentityUser<UniqueObjectId>, IHasId
{
	public UniqueObjectId? AssociatedAuthorId { get; set; }
	public TblAuthor? AssociatedAuthor { get; set; }
}
