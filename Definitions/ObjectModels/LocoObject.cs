using Definitions.ObjectModels.Types;

namespace Definitions.ObjectModels;

public class LocoObject
{
	public LocoObject(ObjectType objectType, ILocoStruct obj, StringTable stringTable, List<GraphicsElement> graphicsElements)
	{
		ObjectType = objectType;
		Object = obj;
		StringTable = stringTable;
		ImageTable = new ImageTable
		{
			Groups = [("All", graphicsElements)]
		};
	}

	public ObjectType ObjectType { get; init; }
	public ILocoStruct Object { get; set; }
	public StringTable StringTable { get; set; }
	public ImageTable ImageTable { get; set; }

	//public List<GraphicsElement> GraphicsElements
	//{ get => ImageTable.GraphicsElements; set => throw new NotImplementedException(); }
}
