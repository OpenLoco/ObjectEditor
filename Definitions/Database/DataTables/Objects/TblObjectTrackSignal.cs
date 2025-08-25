using Definitions.ObjectModels.Objects.TrackSignal;

namespace Definitions.Database;

public class TblObjectTrackSignal : DbSubObject, IConvertibleToTable<TblObjectTrackSignal, TrackSignalObject>
{
	public TrackSignalObjectFlags Flags { get; set; }
	public uint8_t AnimationSpeed { get; set; }
	public uint8_t NumFrames { get; set; }
	public int16_t BuildCostFactor { get; set; }
	public int16_t SellCostFactor { get; set; }
	public uint8_t CostIndex { get; set; }
	public uint16_t DesignedYear { get; set; }
	public uint16_t ObsoleteYear { get; set; }

	//public ICollection<UniqueObjectId> CompatibleTrack { get; set; }
	//public uint8_t var_0B { get; set; }

	public static TblObjectTrackSignal FromObject(TblObject tbl, TrackSignalObject obj)
		=> new()
		{
			Parent = tbl,
			Flags = obj.Flags,
			AnimationSpeed = obj.AnimationSpeed,
			NumFrames = obj.NumFrames,
			BuildCostFactor = obj.BuildCostFactor,
			SellCostFactor = obj.SellCostFactor,
			CostIndex = obj.CostIndex,
			DesignedYear = obj.DesignedYear,
			ObsoleteYear = obj.ObsoleteYear,
		};
}
