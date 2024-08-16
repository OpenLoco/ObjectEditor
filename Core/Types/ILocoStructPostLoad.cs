using System.ComponentModel;

namespace OpenLoco.Dat.Types
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public interface ILocoStructPostLoad
	{
		void PostLoad();
	}
}
