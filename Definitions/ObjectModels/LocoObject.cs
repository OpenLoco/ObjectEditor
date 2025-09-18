using Definitions.ObjectModels.Types;

namespace Definitions.ObjectModels;

public class LocoObject
{
	public LocoObject(ObjectType objectType, ILocoStruct obj, StringTable stringTable, ImageTable? imageTable = null)
	{
		ObjectType = objectType;
		Object = obj;
		StringTable = stringTable;
		ImageTable = imageTable;
	}

	public ObjectType ObjectType { get; init; }
	public ILocoStruct Object { get; set; }
	public StringTable StringTable { get; set; }

	public ImageTable? ImageTable { get; set; }
}

//public class LocoObjectWithGraphics : LocoObject
//{
//	public LocoObjectWithGraphics(ObjectType objectType, ILocoStruct obj, StringTable stringTable, ImageTable imageTable)
//		: base(objectType, obj, stringTable)
//	{
//		ImageTable = imageTable;
//	}

//	public ImageTable ImageTable { get; set; }
//}

