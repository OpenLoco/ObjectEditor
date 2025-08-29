using System.ComponentModel;

namespace Definitions.ObjectModels.Objects.Vehicle;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class SimpleAnimation : ILocoStruct
{
	public object_id ObjectId { get; set; }
	public uint8_t Height { get; set; }
	public SimpleAnimationType Type { get; set; }

	public bool Validate() => true;
}
