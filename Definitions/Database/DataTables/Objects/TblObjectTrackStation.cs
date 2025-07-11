using Dat.Objects;

namespace Definitions.Database
{
	public class TblObjectTrackStation : DbSubObject, IConvertibleToTable<TblObjectTrackStation, TrackStationObject>
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

		//public ICollection<UniqueObjectId> CompatibleTrack { get; set; } // only used for runtime loco { get; set; } this isn't part of object 'definition'
		//public ICollection<uint8_t> CargoOffsetBytes { get; set; }
		//public ICollection<uint8_t> ManualPower { get; set; }
		//public uint8_t var_0B { get; set; }
		//public uint8_t var_0D { get; set; }

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
			};
	}
}
