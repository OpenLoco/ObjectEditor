using Dat.Data;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Types;

namespace Dat.Loaders;

//public interface IDatDetails
//{
//	public static abstract int DatStructSize { get; }
//	public static abstract ObjectType ModelObjectType { get; }
//	public static abstract DatObjectType DatObjectType { get; }
//}

public interface IDatObjectLoader //<TDetails> where TDetails : IDatDetails
{
	//public static abstract TDetails DatDetails { get; }

	public static abstract LocoObject Load(MemoryStream stream);
	public static abstract void Save(MemoryStream stream, LocoObject obj);
}
