using System.ComponentModel;

namespace OpenLoco.Dat.Types
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public interface ILocoStruct
	{
		bool Validate();
	}
}
