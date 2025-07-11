using Dat.Objects;

namespace Definitions.Database
{
	public class TblObjectWater : DbSubObject, IConvertibleToTable<TblObjectWater, WaterObject>
	{
		public uint8_t CostIndex { get; set; }
		public int16_t CostFactor { get; set; }
		//public uint8_t var_03 { get; set; }

		public static TblObjectWater FromObject(TblObject tbl, WaterObject obj)
			=> new()
			{
				Parent = tbl,
				CostIndex = obj.CostIndex,
				CostFactor = obj.CostFactor,
			};
	}
}
