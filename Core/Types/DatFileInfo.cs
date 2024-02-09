using System.ComponentModel;
using OpenLoco.ObjectEditor.Headers;

namespace OpenLoco.ObjectEditor.DatFileParsing
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[Category("Header")]
	public record DatFileInfo(S5Header S5Header, ObjectHeader ObjectHeader);
}
