using System.Diagnostics.CodeAnalysis;

namespace Definitions.ObjectModels.Objects.Wall;

public class WallObject : ILocoStruct, IImageTableNameProvider
{
	public uint8_t Height { get; set; }
	public uint8_t ToolId { get; set; } // unused in loco???
	public WallObjectFlags1 Flags1 { get; set; } = WallObjectFlags1.None;
	public WallObjectFlags2 Flags2 { get; set; } = WallObjectFlags2.None; // unused in loco???

	public bool Validate() => true;

	public bool TryGetImageName(int id, [MaybeNullWhen(false)] out string value)
	{
		var result = ImageIdNameMap.TryGetValue(id, out value);

		if (id is >= 0 and <= 5 && Flags1.HasFlag(WallObjectFlags1.DoubleSided))
		{
			value = $"{value} back";
		}

		if (id is >= 6 and <= 11 && Flags1.HasFlag(WallObjectFlags1.HasGlass))
		{
			value = $"{value} glass overlay";
		}
		else
		{
			value = $"{value} front";
		}

		return result;
	}

	public static Dictionary<int, string> ImageIdNameMap = new()
	{
		{ 0, "Flat SE" },
		{ 1, "Flat NE" },
		{ 2, "Sloped SE" },
		{ 3, "Sloped NE" },
		{ 4, "Sloped NW" },
		{ 5, "Sloped SW" },
		{ 6, "Flat SE" },
		{ 7, "Flat NE" },
		{ 8, "Sloped SE" },
		{ 9, "Sloped NE" },
		{ 10, "Sloped NW" },
		{ 11, "Sloped SW" },
	};
}
