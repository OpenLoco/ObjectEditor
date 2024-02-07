using System.ComponentModel;

namespace OpenLocoObjectEditor.DatFileParsing
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public interface ILocoStruct
	{ }
}
