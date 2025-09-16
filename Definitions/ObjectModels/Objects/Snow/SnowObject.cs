using System.Diagnostics.CodeAnalysis;

namespace Definitions.ObjectModels.Objects.Snow;
public class SnowObject : ILocoStruct, IImageTableNameProvider
{
	public bool Validate() => true;

	public bool TryGetImageName(int id, [MaybeNullWhen(false)] out string value)
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
