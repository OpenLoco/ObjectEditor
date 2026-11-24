using Definitions.Database;
using Definitions.ObjectModels.Objects.Shared;
using Definitions.ObjectModels.Objects.Track;
using Definitions.ObjectModels.Objects.TrackStation;
using Definitions.ObjectModels.Types;

namespace Definitions.DTO;

public class DtoObjectTrackStation : IDtoSubObject
{
	public uint8_t PaintStyle { get; set; }
	public uint8_t Height { get; set; }
	public int16_t BuildCostFactor { get; set; }
	public int16_t SellCostFactor { get; set; }
	public uint8_t CostIndex { get; set; }
	public TrackStationObjectFlags Flags { get; set; }
	public uint16_t DesignedYear { get; set; }
	public uint16_t ObsoleteYear { get; set; }
	public TrackTraitFlags TrackPieces { get; set; }
	public uint8_t PlatformType { get; set; }
	public List<ObjectModelHeader> CompatibleTrackObjects { get; set; } = [];
	public CargoOffset[][][] CargoOffsets { get; set; } = [];
	public uint8_t[][] DiagonalCargoOffsetBytes { get; set; } = [];
	public UniqueObjectId Id { get; set; }
}
