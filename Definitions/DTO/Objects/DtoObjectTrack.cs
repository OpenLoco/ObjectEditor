using OpenLoco.Dat.Objects;

namespace OpenLoco.Definitions.DTO
{
    public class DtoObjectTrack : IHasId
    {
        public TrackTraitFlags TrackPieces {get; set;}
        public TrackTraitFlags StationTrackPieces {get; set;}
        public int16_t BuildCostFactor {get; set;}
        public int16_t SellCostFactor {get; set;}
        public int16_t TunnelCostFactor {get; set;}
        public uint8_t CostIndex {get; set;}
        public Speed16 CurveSpeed {get; set;}
        public TrackObjectFlags Flags {get; set;}
        public uint8_t DisplayOffset {get; set;}
        public UniqueObjectId Id {get; set;}
    }
}
