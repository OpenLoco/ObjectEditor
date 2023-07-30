using System.ComponentModel;

namespace OpenLocoTool
{
	public interface ILocoObject
	{
		public DatFileHeader DatFileHeader { get; set; }
		public ObjHeader ObjHeader { get; set; }
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class LocoObject<T> : ILocoObject where T : struct
	{
		public LocoObject(DatFileHeader datHdr, ObjHeader objHdr, T obj)
		{
			DatFileHeader = datHdr;
			ObjHeader = objHdr;
			Object = obj;
		}

		public DatFileHeader DatFileHeader { get; set; }
		public ObjHeader ObjHeader { get; set; }
		public T Object { get; set; }

		//protected virtual T DataAs<T>() where T : struct
		//	=> MemoryMarshal.Read<T>(dataSpan);
	}
}
