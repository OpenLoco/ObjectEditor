using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Types;
using System.ComponentModel;

namespace OpenLoco.Dat.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x04)]
	public record AirportBuilding(
		[property: LocoStructOffset(0x00)] uint8_t Index,
		[property: LocoStructOffset(0x01)] uint8_t Rotation,
		[property: LocoStructOffset(0x02)] int8_t X,
		[property: LocoStructOffset(0x03)] int8_t Y
		) : ILocoStruct
	{
		public AirportBuilding() : this(0, 0, 0, 0)
		{ }

		public bool Validate()
			=> true;
	}
}
