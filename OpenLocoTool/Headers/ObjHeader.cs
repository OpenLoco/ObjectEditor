using System.ComponentModel;
using System.Runtime.InteropServices;

namespace OpenLocoTool
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record ObjHeader(SawyerEncoding Encoding, uint32_t Length);
}
