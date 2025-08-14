using Definitions.ObjectModels.Types;

namespace Definitions.ObjectModels;

public interface IHasGraphicsElements
{
	public List<GraphicsElement> GraphicsElements { get; set; }
}

public interface IImageTableNameProvider
{
	public bool TryGetImageName(int id, out string? value);
}

public class DefaultImageTableNameProvider : IImageTableNameProvider
{
	public bool TryGetImageName(int id, out string? value)
	{
		value = id.ToString();
		return true;
	}
}
