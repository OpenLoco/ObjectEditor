using System.ComponentModel;

namespace Definitions.ObjectModels.Types;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class Pos2(coord_t x, coord_t y)
{
	public coord_t X { get; } = x;
	public coord_t Y { get; } = y;

	public static Pos2 Zero => new(0, 0);
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public class Pos3(coord_t x, coord_t y, coord_t z)
{
	public coord_t X { get; } = x;
	public coord_t Y { get; } = y;
	public coord_t Z { get; } = z;

	public static Pos3 Zero => new(0, 0, 0);
}
