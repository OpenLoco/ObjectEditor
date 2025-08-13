using Definitions.ObjectModels.Objects.Streetlight;

namespace Definitions.Database;

public class TblObjectStreetLight : DbSubObject, IConvertibleToTable<TblObjectStreetLight, StreetLightObject>
{
	//public ICollection<uint16_t> DesignedYears { get; set; }

	public static TblObjectStreetLight FromObject(TblObject tbl, StreetLightObject obj)
		=> new()
		{
			Parent = tbl,
		};
}
