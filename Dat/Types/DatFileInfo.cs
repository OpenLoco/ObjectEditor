using System.ComponentModel;

namespace Dat.Types
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record DatFileInfo(S5Header S5Header, ObjectHeader ObjectHeader);
}
