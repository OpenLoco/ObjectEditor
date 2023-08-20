using System.ComponentModel;
using OpenLocoTool.Headers;

namespace OpenLocoTool.DatFileParsing
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public interface ILocoStruct
	{
		static int StructLength { get; }
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public interface ILocoObject
	{
		ObjectHeader ObjectHeader { get; set; }
		ILocoStruct Object { get; set; }
		StringTable StringTable { get; set; }
		G1Header G1Header { get; set; }
		List<G1Element32> G1Elements { get; set; }
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class LocoObject : ILocoObject
	{
		public LocoObject(ObjectHeader objHdr, ILocoStruct obj, StringTable stringTable, G1Header g1Header, List<G1Element32> g1Elements)
		{
			ObjectHeader = objHdr;
			Object = obj;
			StringTable = stringTable;
			G1Header = g1Header;
			G1Elements = g1Elements;
		}
		public LocoObject(ObjectHeader objHdr, ILocoStruct obj, StringTable stringTable)
		{
			ObjectHeader = objHdr;
			Object = obj;
			StringTable = stringTable;
			G1Header = null;
			G1Elements = null;
		}

		public ObjectHeader ObjectHeader { get; set; }
		public ILocoStruct Object { get; set; }
		public StringTable StringTable { get; set; }
		public G1Header? G1Header { get; set; }
		public List<G1Element32>? G1Elements { get; set; }
	}
}
