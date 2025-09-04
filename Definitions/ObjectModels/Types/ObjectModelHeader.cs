using System.ComponentModel;

namespace Definitions.ObjectModels.Types;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class ObjectModelHeader(string name, uint checksum, ObjectType objectType, ObjectSource objectSource)
{
	public string Name { get; set; } = name;
	public uint Checksum { get; set; } = checksum;
	public ObjectType ObjectType { get; set; } = objectType;
	public ObjectSource ObjectSource { get; set; } = objectSource;
}
