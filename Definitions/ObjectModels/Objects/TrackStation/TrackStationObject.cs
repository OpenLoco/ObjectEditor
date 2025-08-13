using Definitions.ObjectModels.Objects.Track;

namespace Definitions.ObjectModels.Objects.TrackStation;

public class TrackStationObject : ILocoStruct
{
	public uint8_t PaintStyle { get; set; }
	public uint8_t Height { get; set; }
	public TrackTraitFlags TrackPieces { get; set; }
	public int16_t BuildCostFactor { get; set; }
	public int16_t SellCostFactor { get; set; }
	public uint8_t CostIndex { get; set; }
	public TrackStationObjectFlags Flags { get; set; }
	public uint16_t DesignedYear { get; set; }
	public uint16_t ObsoleteYear { get; set; }

	public bool Validate() => throw new NotImplementedException();
}
