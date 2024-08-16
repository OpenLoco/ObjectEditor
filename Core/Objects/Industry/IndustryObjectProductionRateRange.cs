using System.ComponentModel;
using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Types;

namespace OpenLoco.Dat.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x04)]

	public record IndustryObjectProductionRateRange(
		[property: LocoStructOffset(0x00)] uint16_t Min,
		[property: LocoStructOffset(0x02)] uint16_t Max
		) : ILocoStruct
	{
		public bool Validate() => true;
	}
}
