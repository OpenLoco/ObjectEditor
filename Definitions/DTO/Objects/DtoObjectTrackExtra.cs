using Definitions.Database;
using Definitions.ObjectModels.Objects.Track;

namespace Definitions.DTO;

public class DtoObjectTrackExtra : IDtoSubObject
{
	public uint8_t PaintStyle { get; set; }
	public uint8_t CostIndex { get; set; }
	public int16_t BuildCostFactor { get; set; }
	public int16_t SellCostFactor { get; set; }
	public TrackTraitFlags TrackPieces { get; set; }
	public UniqueObjectId Id { get; set; }
}
