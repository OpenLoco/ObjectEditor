using Definitions.ObjectModels.Types;
using System.ComponentModel;

namespace Definitions.ObjectModels.Objects.Shared;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class CargoOffset
{
	public Pos3 A { get; set; } = new();
	public Pos3 B { get; set; } = new();
}
