using System.ComponentModel;

namespace Definitions.ObjectModels.Types;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class ObjectModelHeader(string name, uint checksum, ObjectType objectType, ObjectSource objectSource)
{
	public string Name { get; } = name;
	public uint Checksum { get; } = checksum;
	public ObjectType ObjectType { get; } = objectType;
	public ObjectSource ObjectSource { get; } = objectSource;
}
