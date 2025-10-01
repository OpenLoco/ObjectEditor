using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Definitions.ObjectModels.Types;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class ObjectModelHeader(string name, ObjectType objectType, ObjectSource objectSource, uint datchecksum) : ILocoStruct
{
	public ObjectModelHeader()
		: this(string.Empty, ObjectType.Airport, ObjectSource.Custom, 0)
	{ }

	public string Name { get; set; } = name;
	public ObjectType ObjectType { get; set; } = objectType;
	public ObjectSource ObjectSource { get; set; } = objectSource;
	public uint DatChecksum { get; set; } = datchecksum;

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		=> [];
}
