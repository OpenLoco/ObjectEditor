using Dat.Objects;

namespace Definitions.Database
{
	public class TblObjectRoad : DbSubObject, IConvertibleToTable<TblObjectRoad, RoadObject>
	{
		public RoadTraitFlags RoadPieces { get; set; }
		public int16_t BuildCostFactor { get; set; }
		public int16_t SellCostFactor { get; set; }
		public int16_t TunnelCostFactor { get; set; }
		public uint8_t CostIndex { get; set; }
		public Speed16 MaxSpeed { get; set; }
		public RoadObjectFlags Flags { get; set; }
		public uint8_t PaintStyle { get; set; }
		public uint8_t DisplayOffset { get; set; }
		public TownSize TargetTownSize { get; set; }

		//public TblObjectTunnel Tunnel { get; set; }
		//public uint16_t _CompatibleRoads { get; set; } // bitset
		//public uint16_t _CompatibleTracks { get; set; } // bitset
		//public ICollection<TblBridgeObject> Bridges { get; set; }
		//public ICollection<TblRoadExtraObject> Mods { get; set; }
		//public ICollection<TblRoadObject> RoadsAndTracks { get; set; }
		//public ICollection<TblRoadStationObject> Stations { get; set; }

		public static TblObjectRoad FromObject(TblObject tbl, RoadObject obj)
			=> new()
			{
				Parent = tbl,
				RoadPieces = obj.RoadPieces,
				BuildCostFactor = obj.BuildCostFactor,
				SellCostFactor = obj.SellCostFactor,
				TunnelCostFactor = obj.TunnelCostFactor,
				CostIndex = obj.CostIndex,
				MaxSpeed = obj.MaxSpeed,
				Flags = obj.Flags,
				PaintStyle = obj.PaintStyle,
				DisplayOffset = obj.DisplayOffset,
				TargetTownSize = obj.TargetTownSize,
				//Tunnel = obj.Tunnel,
			};
	}
}
