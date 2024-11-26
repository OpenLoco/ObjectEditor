using OpenLoco.Dat.FileParsing;
using System.ComponentModel;

namespace OpenLoco.Dat.Types.SCV5
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(StructLength)]
	public record S5FileHeader(
		[property: LocoStructOffset(0x00)] S5Type Type,
		[property: LocoStructOffset(0x01)] HeaderFlags Flags,
		[property: LocoStructOffset(0x02)] uint16_t NumPackedObjects,
		[property: LocoStructOffset(0x04)] uint32_t Version,
		[property: LocoStructOffset(0x08)] uint32_t Magic,
		[property: LocoStructOffset(0x0C), LocoArrayLength(20)] byte[] Padding)
		: ILocoStruct
	{
		public const int StructLength = 0x20;
		public bool Validate() => true;
	}
}
