using Definitions.ObjectModels.Types;
using System.Diagnostics.CodeAnalysis;

namespace Definitions.ObjectModels;

public interface IHasGraphicsElements
{
	List<GraphicsElement> GraphicsElements { get; set; } // todo: probably change to IEnumerable
}

public interface IImageTableNameProvider
{
	bool TryGetImageName<T>(T model, int id, [MaybeNullWhen(false)] out string value) where T : ILocoStruct;
}

public interface IImageTableNameProvider<T> where T : ILocoStruct
{
	bool TryGetImageName(T model, int id, [MaybeNullWhen(false)] out string value);
}

public abstract class ImageTableNamer<T> : IImageTableNameProvider, IImageTableNameProvider<T> where T : ILocoStruct
{
	public abstract bool TryGetImageName(T model, int id, [MaybeNullWhen(false)] out string value);

	public bool TryGetImageName<T1>(T1 model, int id, [MaybeNullWhen(false)] out string value) where T1 : ILocoStruct
	{
		if (model is T tModel)
		{
			return TryGetImageName(tModel, id, out value); // call your specialized method
		}

		value = DefaultImageTableNameProvider.GetImageName(id);
		return false;
	}
}

public class DefaultImageTableNameProvider : IImageTableNameProvider
{
	public static DefaultImageTableNameProvider Instance { get; } = new();

	public bool TryGetImageName<T>(T model, int id, [MaybeNullWhen(false)] out string value) where T : ILocoStruct
	{
		value = GetImageName(id);
		return true;
	}

	public static string GetImageName(int id)
		=> $"{id}-unnamed";
}
