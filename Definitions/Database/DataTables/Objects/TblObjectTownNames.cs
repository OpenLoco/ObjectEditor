using OpenLoco.Dat.Objects;

namespace OpenLoco.Definitions.Database
{
	public class TblObjectTownNames : DbSubObject, IConvertibleToTable<TblObjectTownNames, TownNamesObject>
	{
		//public ICollection<Category> Categories { get; set; }
		public static TblObjectTownNames FromObject(TblObject tbl, TownNamesObject obj)
			=> new()
			{
				Parent = tbl,
			};
	}
}
