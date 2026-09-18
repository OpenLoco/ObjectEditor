using Definitions.ObjectModels.Objects.Shared;
using Definitions.ObjectModels.Objects.Track;
using Definitions.ObjectModels.Objects.TrackStation;
using Definitions.ObjectModels.Types;

namespace Definitions.Database;

public class TblObjectTrackStation : DbSubObject, IConvertibleToTable<TblObjectTrackStation, TrackStationObject>
{
	public uint8_t PaintStyle { get; set; }
	public uint8_t Height { get; set; }
	public TrackTraitFlags TrackPieces { get; set; }
	public int16_t BuildCostFactor { get; set; }
	public int16_t SellCostFactor { get; set; }
	public uint8_t CostIndex { get; set; }
	public uint8_t PlatformType { get; set; } // 0 = terminus, 1 = always uses the middle platform image, 2+ = only uses it when connected at both ends
	public TrackStationObjectFlags Flags { get; set; }
	public uint16_t DesignedYear { get; set; }
	public uint16_t ObsoleteYear { get; set; }

	public List<ObjectModelHeader> CompatibleTrackObjects { get; set; } = [];
	public CargoOffset[][][] CargoOffsets { get; set; } = [];
	public uint8_t[][] DiagonalCargoOffsetBytes { get; set; } = []; // parsed in the same way as CargoOffsets but never read

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
			Flags = obj.Flags,
			DesignedYear = obj.DesignedYear,
			ObsoleteYear = obj.ObsoleteYear,
			CompatibleTrackObjects = obj.CompatibleTrackObjects,
			CargoOffsets = obj.CargoOffsets,
			DiagonalCargoOffsetBytes = obj.DiagonalCargoOffsetBytes,
		};
}
