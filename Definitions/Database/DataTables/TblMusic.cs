using Microsoft.EntityFrameworkCore;

namespace Definitions.Database;

/// <summary>A music file dropped into <c>GameData/Music</c>.</summary>
[Index(nameof(Name), IsUnique = true)]
public class TblMusic : DbCoreObject
{
}
