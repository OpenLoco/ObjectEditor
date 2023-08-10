using System.ComponentModel;
using OpenLocoTool.DatFileParsing;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record VehicleObjectUnk(
		[property: LocoStructProperty(0x00)] uint8_t Length,
		[property: LocoStructProperty(0x01)] uint8_t var_01,
		[property: LocoStructProperty(0x02)] uint8_t FrontBogieSpriteInd, // index of a bogieSprites struct
		[property: LocoStructProperty(0x03)] uint8_t BackBogieSpriteInd, // index of a bogieSprites struct
		[property: LocoStructProperty(0x04)] uint8_t BodySpriteInd, // index of a bogieSprites struct
		[property: LocoStructProperty(0x05)] uint8_t var_05
		) : ILocoStruct
	{
		public static int ObjectStructSize => 0x6;
	}
}
