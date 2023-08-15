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
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class LocoObject : ILocoObject
	{
		public LocoObject(ObjectHeader objHdr, ILocoStruct obj, Dictionary<LocoLanguageId, string> stringTable, List<G1Element32> g1Elements)
		{
			ObjectHeader = objHdr;
			Object = obj;
			StringTable = stringTable;
			G1Elements = g1Elements;
		}

		public ObjectHeader ObjectHeader { get; set; }
		public ILocoStruct Object { get; set; }

		public Dictionary<LocoLanguageId, string> StringTable { get; set; }

		public List<G1Element32> G1Elements { get; set; }
	}
}
