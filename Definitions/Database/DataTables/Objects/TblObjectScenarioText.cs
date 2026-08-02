using Definitions.Database.Base;
using Definitions.ObjectModels.Objects.ScenarioText;

namespace Definitions.Database.DataTables.Objects;

public class TblObjectScenarioText : DbSubObject, IConvertibleToTable<TblObjectScenarioText, ScenarioTextObject>
{
	public static TblObjectScenarioText FromObject(TblObject tbl, ScenarioTextObject obj)
		=> new()
		{
			Parent = tbl,
		};
}
