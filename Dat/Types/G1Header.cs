using Dat.FileParsing;
using System.ComponentModel;

namespace Dat.Types
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x08)]
	public record G1Header(
		[property: LocoStructOffset(0x00)] uint32_t NumEntries,
		[property: LocoStructOffset(0x04)] uint32_t TotalSize
		) : ILocoStruct
	{
		public static int StructLength => 0x08;
		public byte[] ImageData = [];

		public bool Validate() => true;
	}
}
