
using OpenLoco.Dat.Data;
using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Types;
using System.ComponentModel;

namespace OpenLoco.Dat.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x12)]
	[LocoStructType(ObjectType.LevelCrossing)]
	[LocoStringTable("Name")]
	public record LevelCrossingObject(
		[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
		[property: LocoStructOffset(0x02)] int16_t CostFactor,
		[property: LocoStructOffset(0x04)] int16_t SellCostFactor,
		[property: LocoStructOffset(0x06)] uint8_t CostIndex,
		[property: LocoStructOffset(0x07)] uint8_t AnimationSpeed,
		[property: LocoStructOffset(0x08)] uint8_t ClosingFrames,
		[property: LocoStructOffset(0x09)] uint8_t ClosedFrames,
		[property: LocoStructOffset(0x0A)] uint8_t var_0A, // something like IdleAnimationFrames or something
		[property: LocoStructOffset(0x0B), LocoPropertyMaybeUnused] uint8_t pad_0B,
		[property: LocoStructOffset(0x0C)] uint16_t DesignedYear,
		[property: LocoStructOffset(0x0E), Browsable(false)] image_id Image
		) : ILocoStruct
	{
		public bool Validate()
		{
			if (-SellCostFactor > CostFactor)
			{
				return false;
			}

			if (CostFactor <= 0)
			{
				return false;
			}

			return ClosingFrames switch
			{
				1 or 2 or 4 or 8 or 16 or 32 => true,
				_ => false,
			};
		}
	}
}
