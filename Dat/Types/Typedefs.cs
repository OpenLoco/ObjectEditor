using OpenLoco.Dat.FileParsing;
using System.ComponentModel;

namespace OpenLoco.Dat.Types
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x04)]
	public record Pos2(
		[property: LocoStructOffset(0x00)] coord_t X = 0,
		[property: LocoStructOffset(0x02)] coord_t Y = 0
		) : ILocoStruct
	{
		public bool Validate() => true;
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x06)]
	public record Pos3(
		[property: LocoStructOffset(0x00)] coord_t X = 0,
		[property: LocoStructOffset(0x02)] coord_t Y = 0,
		[property: LocoStructOffset(0x04)] coord_t Z = 0
		) : ILocoStruct
	{
		public bool Validate() => true;
	}
}
