using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Definitions.ObjectModels.Objects.Vehicle;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class VehicleObjectCar : ILocoStruct
{
	public uint8_t FrontBogiePosition { get; set; }
	public uint8_t BackBogiePosition { get; set; }
	public uint8_t FrontBogieSpriteIndex { get; set; }
	public uint8_t BackBogieSpriteIndex { get; set; }
	public uint8_t BodySpriteIndex { get; set; }
	public uint8_t var_05 { get; set; }

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		=> [];
}
