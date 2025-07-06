using OpenLoco.Dat.Objects;

namespace OpenLoco.Definitions.Database
{
	public class TblObjectScenarioText : DbSubObject, IConvertibleToTable<TblObjectScenarioText, ScenarioTextObject>
	{
		public static TblObjectScenarioText FromObject(TblObject tbl, ScenarioTextObject obj)
			=> new()
			{
				Parent = tbl,
			};
	}
}
