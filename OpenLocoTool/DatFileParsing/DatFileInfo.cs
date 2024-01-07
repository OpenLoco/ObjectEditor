using System.ComponentModel;
using OpenLocoTool.Headers;

namespace OpenLocoTool.DatFileParsing
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class DatFileInfo(S5Header s5Header, ObjectHeader objectHeader)
	{
		public S5Header S5Header { get; set; } = s5Header;
		public ObjectHeader ObjectHeader { get; set; } = objectHeader;
	}
}
