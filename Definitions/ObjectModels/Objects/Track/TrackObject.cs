namespace Definitions.ObjectModels.Objects.Track;

public class TrackObject : ILocoStruct
{
	public TrackTraitFlags TrackPieces { get; set; }
	public TrackTraitFlags StationTrackPieces { get; set; }
	public int16_t BuildCostFactor { get; set; }
	public int16_t SellCostFactor { get; set; }
	public int16_t TunnelCostFactor { get; set; }
	public uint8_t CostIndex { get; set; }
	public Speed16 CurveSpeed { get; set; }
	public TrackObjectFlags Flags { get; set; }
	public uint8_t DisplayOffset { get; set; }

	public bool Validate() => throw new NotImplementedException();
}
