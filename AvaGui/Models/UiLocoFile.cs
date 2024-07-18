using OpenLoco.ObjectEditor.DatFileParsing;
using System.ComponentModel;

namespace AvaGui.Models
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class UiLocoFile : IUiObject
	{
		public DatFileInfo DatFileInfo { get; set; }
		public ILocoObject LocoObject { get; set; }
	}

	//[TypeConverter(typeof(ExpandableObjectConverter))]
	//public class UILocoObject
	//{
	//	public UILocoObject(ILocoStruct @struct, StringTable stringTable, BindingList<G1Element32> g1Elements)
	//	{
	//		Struct = @struct;
	//		StringTable = stringTable;
	//		G1Elements = g1Elements;
	//	}

	//	public ILocoStruct Struct { get; set; }
	//	public StringTable StringTable { get; set; }

	//	[TypeConverter(typeof(TypeListConverter))]
	//	public BindingList<G1Element32> G1Elements { get; set; }
	//}
}
