using Definitions.ObjectModels.Types;

namespace Definitions.ObjectModels;

public class LocoObject
{
	public LocoObject(ObjectType objectType, ILocoStruct obj, StringTable stringTable, List<GraphicsElement> graphicsElements)
		: this(objectType, obj, stringTable, new ImageTable { Groups = [("All", graphicsElements)] })
	{ }

	public LocoObject(ObjectType objectType, ILocoStruct obj, StringTable stringTable, ImageTable imageTable)
	{
		ObjectType = objectType;
		Object = obj;
		StringTable = stringTable;
		ImageTable = imageTable;
	}

	public ObjectType ObjectType { get; init; }
	public ILocoStruct Object { get; set; }
	public StringTable StringTable { get; set; }
	public ImageTable ImageTable { get; set; }
}
