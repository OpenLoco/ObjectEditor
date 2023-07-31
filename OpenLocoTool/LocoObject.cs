using System.ComponentModel;

namespace OpenLocoTool
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public interface ILocoSubObject//<T>
	{
		//T Read(ReadOnlySpan<byte> data);
		ReadOnlySpan<byte> Write();

		int BinarySize { get; }
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public interface ILocoObject
	{
		DatFileHeader DatFileHeader { get; set; }
		ObjHeader ObjHeader { get; set; }
		ILocoSubObject Object { get; set; }
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class LocoObject : ILocoObject
	{
		public LocoObject(DatFileHeader datHdr, ObjHeader objHdr, ILocoSubObject obj)
		{
			DatFileHeader = datHdr;
			ObjHeader = objHdr;
			Object = obj;
		}

		public DatFileHeader DatFileHeader { get; set; }
		public ObjHeader ObjHeader { get; set; }
		public ILocoSubObject Object { get; set; }
	}

	public record EmptyObject(string PlaceholderText) : ILocoSubObject
	{
		public ReadOnlySpan<byte> Write() => new byte[1] { 123 };

		public int BinarySize => 1;
	}
}
