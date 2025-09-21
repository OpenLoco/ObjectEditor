using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Definitions.ObjectModels.Types;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class ObjectModelHeader(string name, ObjectType objectType, ObjectSource objectSource, uint datchecksum) : ILocoStruct
{
	public string Name { get; set; } = name;
	public ObjectType ObjectType { get; set; } = objectType;
	public ObjectSource ObjectSource { get; set; } = objectSource;
	public uint DatChecksum { get; set; } = datchecksum;

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		=> [];
}
