namespace Definitions.Database;

public class TblScenarioPack : DbCoreObject
{
	public ICollection<TblScenario> Scenarios { get; set; } = [];
}
