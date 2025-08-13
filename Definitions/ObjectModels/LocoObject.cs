using Definitions.ObjectModels.Types;

namespace Definitions.ObjectModels;

public class LocoObject
{
	public LocoObject(ObjectType objectType, ILocoStruct obj, StringTable stringTable, List<GraphicsElement> graphicsElements)
	{
		ObjectType = objectType;
		Object = obj;
		StringTable = stringTable;
		GraphicsElements = graphicsElements;
	}

	public LocoObject(ObjectType objectType, ILocoStruct obj, StringTable stringTable)
	{
		ObjectType = objectType;
		Object = obj;
		StringTable = stringTable;
		GraphicsElements = [];
	}

	public ObjectType ObjectType { get; init; }
	public ILocoStruct Object { get; set; }
	public StringTable StringTable { get; set; }
	public List<GraphicsElement> GraphicsElements { get; set; }
}
