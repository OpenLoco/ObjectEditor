using Microsoft.EntityFrameworkCore;

namespace Definitions.Database;

/// <summary>A tutorial file dropped into <c>GameData/Tutorials</c>.</summary>
[Index(nameof(Name), IsUnique = true)]
public class TblTutorial : DbCoreObject
{
}
