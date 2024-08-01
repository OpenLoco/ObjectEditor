using OpenLoco.ObjectEditor.Data;
using OpenLoco.ObjectEditor.DatFileParsing;
using OpenLoco.ObjectEditor.Objects;

namespace AvaGui.Models
{
	[LocoStructType(ObjectType.Land)]
	public class LandObjectModel(LandObject original)
	{
		public uint8_t CostIndex { get; set; } = original.CostIndex;
		public LandObjectFlags Flags { get; set; } = original.Flags;
		public object_id CliffEdgeHeader1 { get; set; } = original.CliffEdgeHeader1;
		public object_id CliffEdgeHeader2 { get; set; } = original.CliffEdgeHeader2;
		public int8_t CostFactor { get; set; } = original.CostFactor;
		public uint8_t DistributionPattern { get; set; } = original.DistributionPattern;
		public uint8_t NumVariations { get; set; } = original.NumVariations;
		public uint8_t VariationLikelihood { get; set; } = original.VariationLikelihood;
	}
}
