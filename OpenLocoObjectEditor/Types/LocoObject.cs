using System.ComponentModel;
using OpenLocoObjectEditor.DatFileParsing;
using OpenLocoObjectEditor.Headers;

namespace OpenLocoObjectEditor.Types
{

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class LocoObject : ILocoObject
	{
		public LocoObject(ILocoStruct obj, StringTable stringTable, List<G1Element32>? g1Elements)
		{
			Object = obj;
			StringTable = stringTable;
			G1Elements = g1Elements;
		}
		public LocoObject(ILocoStruct obj, StringTable stringTable)
		{
			Object = obj;
			StringTable = stringTable;
			G1Elements = null;
		}

		public ILocoStruct Object { get; set; }
		public StringTable StringTable { get; set; }
		public List<G1Element32>? G1Elements { get; set; }
	}
}
