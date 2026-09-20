using Microsoft.EntityFrameworkCore;

namespace Definitions.Database;

/// <summary>A graphics file dropped into <c>GameData/Graphics</c>.</summary>
[Index(nameof(Name), IsUnique = true)]
public class TblGraphics : DbCoreObject
{
}
