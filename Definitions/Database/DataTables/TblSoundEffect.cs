using Microsoft.EntityFrameworkCore;

namespace Definitions.Database;

/// <summary>A sound effect file dropped into <c>GameData/SoundEffects</c>.</summary>
[Index(nameof(Name), IsUnique = true)]
public class TblSoundEffect : DbCoreObject
{
}
