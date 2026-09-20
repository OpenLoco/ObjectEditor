namespace Definitions.Database;

// scenarios and landscapes, but no savegames
public class TblScenario : DbCoreObject
{
	public ICollection<TblScenarioPack> ScenarioPacks { get; set; } = [];
}
