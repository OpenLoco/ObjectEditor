using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Types;
using System.ComponentModel;

namespace OpenLoco.Dat.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x04)]

	public record IndustryObjectProductionRateRange(
		[property: LocoStructOffset(0x00)] uint16_t Min,
		[property: LocoStructOffset(0x02)] uint16_t Max
		) : ILocoStruct
	{
		public IndustryObjectProductionRateRange() : this(0, 0)
		{ }

		public bool Validate()
			=> true;
	}
}
