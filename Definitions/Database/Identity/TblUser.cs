using Microsoft.AspNetCore.Identity;

namespace Definitions.Database;

public class TblUser : IdentityUser<UniqueObjectId>, IHasId
{
	// FK only - the navigation property is intentionally omitted because TblAuthor lives in
	// LocoDbContext while TblUser lives in IdentityContext. Cross-context navigations are
	// not supported; consumers should resolve the author via LocoDbContext.Authors.Find(...).
	public UniqueObjectId? AssociatedAuthorId { get; set; }
}
