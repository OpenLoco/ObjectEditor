using System.ComponentModel;

namespace Dat.Types;

[TypeConverter(typeof(ExpandableObjectConverter))]
public interface ILocoObject : IHasG1Elements
{
	ILocoStruct Object { get; set; }
	StringTable StringTable { get; set; }
}
