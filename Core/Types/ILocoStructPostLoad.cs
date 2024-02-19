using System.ComponentModel;

namespace OpenLoco.ObjectEditor.DatFileParsing
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public interface ILocoStructPostLoad
	{
		void PostLoad();
	}
}
