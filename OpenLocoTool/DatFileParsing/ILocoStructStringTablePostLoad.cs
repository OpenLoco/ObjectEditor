using System.ComponentModel;

namespace OpenLocoTool.DatFileParsing
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public interface ILocoStructStringTablePostLoad
	{
		void LoadPostStringTable(StringTable stringTable);
	}
}
