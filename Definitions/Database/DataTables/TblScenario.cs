namespace Definitions.Database;

// scenarios and landscapes, but no savegames
public class TblScenario : DbFileObject
{
	public ICollection<TblScenarioPack> ScenarioPacks { get; set; } = [];
}
