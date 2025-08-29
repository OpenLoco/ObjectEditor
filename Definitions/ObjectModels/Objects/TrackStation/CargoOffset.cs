using Definitions.ObjectModels.Types;
using System.ComponentModel;

namespace Definitions.ObjectModels.Objects.TrackStation;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class CargoOffset
{
	public Pos3 A { get; set; }
	public Pos3 B { get; set; }
}
