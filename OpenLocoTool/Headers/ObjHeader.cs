using System.ComponentModel;
using System.Runtime.InteropServices;

namespace OpenLocoTool
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0x5)]
	public struct ObjHeader
	{
		public SawyerEncoding Encoding { get; set; }
		public uint32_t Length { get; set; }
	}
}
