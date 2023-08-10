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
		public LocoObject(ObjectHeader objHdr, ILocoStruct obj)
		{
			ObjectHeader = objHdr;
			Object = obj;
		}

		public ObjectHeader ObjectHeader { get; set; }
		public ILocoStruct Object { get; set; }
	}
}
