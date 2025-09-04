using System.ComponentModel;

namespace Definitions.ObjectModels.Types;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class Pos2
{
	public coord_t X { get; set; }
	public coord_t Y { get; set; }

	public static Pos2 Zero => new Pos2 { X = 0, Y = 0 };
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public class Pos3
{
	public coord_t X { get; set; }
	public coord_t Y { get; set; }
	public coord_t Z { get; set; }

	public static Pos3 Zero => new Pos3 { X = 0, Y = 0, Z = 0 };
}
