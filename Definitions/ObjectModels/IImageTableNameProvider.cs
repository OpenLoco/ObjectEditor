using Definitions.ObjectModels.Types;
using System.Diagnostics.CodeAnalysis;

namespace Definitions.ObjectModels;

public interface IHasGraphicsElements
{
	public List<GraphicsElement> GraphicsElements { get; set; } // todo: probably change to IEnumerable
}

public interface IImageTableNameProvider
{
	public bool TryGetImageName(int id, [MaybeNullWhen(false)] out string value);
}

public class DefaultImageTableNameProvider : IImageTableNameProvider
{
	public bool TryGetImageName(int id, [MaybeNullWhen(false)] out string value)
	{
		value = GetImageName(id);
		return true;
	}

	public static string GetImageName(int id)
		=> $"{id}-unnamed";
}
