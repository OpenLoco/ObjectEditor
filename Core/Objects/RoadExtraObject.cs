using System.ComponentModel;
using OpenLoco.ObjectEditor.Data;
using OpenLoco.ObjectEditor.DatFileParsing;

namespace OpenLoco.ObjectEditor.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x12)]
	[LocoStructType(ObjectType.RoadExtra)]
	[LocoStringTable("Name")]
	public record RoadExtraObject(
		[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
		[property: LocoStructOffset(0x02)] RoadObjectPieceFlags RoadPieces,
		[property: LocoStructOffset(0x04)] uint8_t PaintStyle,
		[property: LocoStructOffset(0x05)] uint8_t CostIndex,
		[property: LocoStructOffset(0x06)] int16_t BuildCostFactor,
		[property: LocoStructOffset(0x08)] int16_t SellCostFactor,
		[property: LocoStructOffset(0x0A), Browsable(false)] image_id Image,
		[property: LocoStructOffset(0x0E), Browsable(false)] image_id var_0E
		) : ILocoStruct
	{
		public bool Validate()
		{
			if (PaintStyle >= 2)
			{
				return false;
			}

			// This check missing from vanilla
			if (CostIndex >= 32)
			{
				return false;
			}

			if (-SellCostFactor > BuildCostFactor)
			{
				return false;
			}
			return BuildCostFactor > 0;
		}
	}
}
