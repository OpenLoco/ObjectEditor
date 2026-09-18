using Definitions.ObjectModels.Objects.Streetlight;

namespace Definitions.Database;

public class TblObjectStreetLight : DbSubObject, IConvertibleToTable<TblObjectStreetLight, StreetLightObject>
{
	public List<uint16_t> DesignedYears { get; set; } = [];

	public static TblObjectStreetLight FromObject(TblObject tbl, StreetLightObject obj)
		=> new()
		{
			Parent = tbl,
			DesignedYears = obj.DesignedYears,
		};
}
