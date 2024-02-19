using System.ComponentModel;
using OpenLoco.ObjectEditor.DatFileParsing;

namespace Core.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x08)]
	public record MovementNode(
		[property: LocoStructOffset(0x00)] int16_t X,
		[property: LocoStructOffset(0x02)] int16_t Y,
		[property: LocoStructOffset(0x04)] int16_t Z,
		[property: LocoStructOffset(0x06)] AirportMovementNodeFlags Flags
		) : ILocoStruct
	{
		public bool Validate() => throw new NotImplementedException();
	}
}
