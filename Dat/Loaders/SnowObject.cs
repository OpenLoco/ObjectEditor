using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using System.ComponentModel;

namespace Dat.Objects;

public abstract class SnowObjectLoader : IDatObjectLoader
{
	public static class Constants
	{ }

	public static class Sizes
	{ }

	public static LocoObject Load(MemoryStream stream) => throw new NotImplementedException();
	public static void Save(MemoryStream ms, LocoObject obj) => throw new NotImplementedException();
}

[LocoStructSize(0x06)]
[LocoStructType(DatObjectType.Snow)]
internal record DatSnowObject(
	[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
	[property: LocoStructOffset(0x02), Browsable(false)] image_id Image
	) : ILocoStruct, IImageTableNameProvider
{
	public bool Validate() => true;

	public bool TryGetImageName(int id, out string? value)
		=> ImageIdNameMap.TryGetValue(id, out value);

	public static Dictionary<int, string> ImageIdNameMap = new()
	{
		{ 0, "surfaceEighthZoom" },
		{ 10, "outlineEighthZoom" },
		{ 19, "surfaceQuarterZoom" },
		{ 38, "outlineQuarterZoom" },
		{ 57, "surfaceHalfZoom" },
		{ 76, "outlineHalfZoom" },
		{ 95, "surfaceFullZoom" },
		{ 114, "outlineFullZoom" },
	};
}
