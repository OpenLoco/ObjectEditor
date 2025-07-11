using System.ComponentModel;

namespace Dat.Types;

[TypeConverter(typeof(ExpandableObjectConverter))]
public interface ILocoStructPostLoad
{
	void PostLoad();
}
