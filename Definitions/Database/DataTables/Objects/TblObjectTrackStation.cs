using Definitions.ObjectModels.Objects.Shared;
using Definitions.ObjectModels.Objects.Track;
using Definitions.ObjectModels.Objects.TrackStation;
using System.Text.Json;

namespace Definitions.Database;

public class TblObjectTrackStation : DbSubObject, IConvertibleToTable<TblObjectTrackStation, TrackStationObject>
{
	public uint8_t PaintStyle { get; set; }
	public uint8_t Height { get; set; }
	public TrackTraitFlags TrackPieces { get; set; }
	public int16_t BuildCostFactor { get; set; }
	public int16_t SellCostFactor { get; set; }
	public uint8_t CostIndex { get; set; }
	public uint8_t var_0B { get; set; }
	public TrackStationObjectFlags Flags { get; set; }
	public uint8_t var_0D { get; set; }
	public uint16_t DesignedYear { get; set; }
	public uint16_t ObsoleteYear { get; set; }
	public string CargoOffsets { get; set; } = "[]";
	public string var_6E { get; set; } = "[]";

	public static TblObjectTrackStation FromObject(TblObject tbl, TrackStationObject obj)
		=> new()
		{
			Parent = tbl,
			PaintStyle = obj.PaintStyle,
			Height = obj.Height,
			TrackPieces = obj.TrackPieces,
			BuildCostFactor = obj.BuildCostFactor,
			SellCostFactor = obj.SellCostFactor,
			CostIndex = obj.CostIndex,
			var_0B = obj.var_0B,
			Flags = obj.Flags,
			var_0D = obj.var_0D,
			DesignedYear = obj.DesignedYear,
			ObsoleteYear = obj.ObsoleteYear,
			CargoOffsets = JsonSerializer.Serialize(obj.CargoOffsets),
			var_6E = JsonSerializer.Serialize(obj.var_6E),
		};
}
