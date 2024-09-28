using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Types;
using System.ComponentModel;

namespace OpenLoco.Dat.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x06)]
	public record VehicleObjectCar(
		[property: LocoStructOffset(0x00)] uint8_t FrontBogiePosition,
		[property: LocoStructOffset(0x01)] uint8_t BackBogiePosition,
		[property: LocoStructOffset(0x02)] uint8_t FrontBogieSpriteIndex,
		[property: LocoStructOffset(0x03)] uint8_t BackBogieSpriteIndex,
		[property: LocoStructOffset(0x04)] uint8_t BodySpriteIndex,
		[property: LocoStructOffset(0x05)] uint8_t var_05
		) : ILocoStruct
	{
		public bool Validate() => true;
	}
}
