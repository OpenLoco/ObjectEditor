using System.ComponentModel;
using OpenLocoObjectEditor.Headers;

namespace OpenLocoObjectEditor.DatFileParsing
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[Category("Header")]
	public record DatFileInfo(S5Header S5Header, ObjectHeader ObjectHeader);
}
