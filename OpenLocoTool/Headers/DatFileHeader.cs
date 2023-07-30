using System.ComponentModel;

namespace OpenLocoTool
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record DatFileHeader(uint32_t Flags, string Name, uint32_t Checksum)
	{
		public byte SourceGame => (byte)((Flags >> 6) & 0x3);
		public ObjectType ObjectType => (ObjectType)(Flags & 0x3F);

		public bool IsCustom => SourceGame == 0;
	}

	//[TypeConverter(typeof(ExpandableObjectConverter))]
	//[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0x10)]
	//public struct DatFileHeader
	//{
	//	public uint Flags { get; set; }
	//	[MarshalAs(UnmanagedType.U1, SizeConst = 8)]
	//	public unsafe fixed byte name[8];
	//	public uint Checksum { get; set; }

	//	//public override string ToString() => $"filename=\"{Encoding.ASCII.GetString(Name)}\" sourceGame={getSourceGame()} type={getType()} isCustom={isCustom()} checksum={Checksum}";

	//	public string Name => GetObjName();

	//	public unsafe string GetObjName()
	//	{
	//		//return Encoding.ASCII.GetString(Name);
	//		fixed (byte* ptr = name)
	//		{
	//			return Encoding.ASCII.GetString(ptr, 8);
	//		}
	//	}

	//	private readonly byte SourceGame => (byte)(Flags >> 6 & 0x3);

	//	public readonly ObjectType ObjectType => (ObjectType)(Flags & 0x3F);

	//	private readonly bool IsCustom => SourceGame == 0;
	//}
}
