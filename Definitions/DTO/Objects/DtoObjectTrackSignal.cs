using Dat.Objects;
using Definitions.Database;

namespace Definitions.DTO;

public class DtoObjectTrackSignal : IDtoSubObject
{
	public TrackSignalObjectFlags Flags { get; set; }
	public uint8_t AnimationSpeed { get; set; }
	public uint8_t NumFrames { get; set; }
	public int16_t BuildCostFactor { get; set; }
	public int16_t SellCostFactor { get; set; }
	public uint8_t CostIndex { get; set; }
	public uint16_t DesignedYear { get; set; }
	public uint16_t ObsoleteYear { get; set; }
	public UniqueObjectId Id { get; set; }
}
