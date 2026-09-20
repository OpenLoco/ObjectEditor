using Microsoft.EntityFrameworkCore;

namespace Definitions.Database;

// scenarios and landscapes, but no savegames
[Index(nameof(Name), IsUnique = true)]
public class TblScenario : DbCoreObject
{
	public ICollection<TblScenarioPack> ScenarioPacks { get; set; } = [];
}
