using Definitions.ObjectModels.Types;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Definitions.ObjectModels.Objects.Vehicle;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class EmitterAnimation : ILocoStruct
{
	public ObjectModelHeader AnimationObject { get; set; } = new ObjectModelHeader() { Name = "<obj>", ObjectSource = ObjectSource.Custom, ObjectType = ObjectType.Steam };
	public uint8_t EmitterVerticalPos { get; set; }
	public EmitterAnimationType Type { get; set; }

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		=> [];
}
