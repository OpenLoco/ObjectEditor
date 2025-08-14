using Definitions.ObjectModels;

namespace Dat.Objects;

public interface IDatObjectLoader
{
	public static abstract LocoObject Load(MemoryStream stream);
	public static abstract void Save(MemoryStream ms, LocoObject obj);
}
