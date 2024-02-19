using System.ComponentModel;
using OpenLoco.ObjectEditor.DatFileParsing;

namespace Core.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x04)]

	public record IndustryObjectProductionRateRange(
		[property: LocoStructOffset(0x00)] uint16_t Min,
		[property: LocoStructOffset(0x02)] uint16_t Max
		) : ILocoStruct
	{
		public bool Validate() => throw new NotImplementedException();
	}
}
