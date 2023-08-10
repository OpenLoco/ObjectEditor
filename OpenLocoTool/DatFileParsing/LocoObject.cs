using System.ComponentModel;
using OpenLocoTool.Headers;

namespace OpenLocoTool.DatFileParsing
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public interface ILocoStruct
	{
		static int ObjectStructSize { get; }
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public interface ILocoObject
	{
		ObjectHeader ObjectHeader { get; set; }
		ObjectHeader2 ObjectHeader2 { get; set; }
		ILocoStruct Object { get; set; }
		string Filename { get; set; }
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class LocoObject : ILocoObject
	{
		public LocoObject(ObjectHeader objHdr, ObjectHeader2 objHdr2, ILocoStruct obj, string filename)
		{
			ObjectHeader = objHdr;
			ObjectHeader2 = objHdr2;
			Object = obj;
			Filename = filename;
		}

		public ObjectHeader ObjectHeader { get; set; }
		public ObjectHeader2 ObjectHeader2 { get; set; }
		public ILocoStruct Object { get; set; }
		public string Filename { get; set; }
	}
}
