using System.Runtime.InteropServices;
using System.Text;

namespace OpenLocoTool
{
	//public enum CustomObj : uint16_t
	//{
	//	Custom = 0,
	//	Standard = 2,
	//}

	//[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0x4)]
	//public struct DatFileHeaderFlags
	//{
	//	public CustomObj CustomObj;
	//	public ObjectType ObjectType;
	//}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0x10)]
	public struct DatFileHeader
	{
		public uint Flags { get; set; }
		[MarshalAs(UnmanagedType.U1, SizeConst = 8)]
		public unsafe fixed byte name[8];
		public uint Checksum { get; set; }

		//public override string ToString() => $"filename=\"{Encoding.ASCII.GetString(Name)}\" sourceGame={getSourceGame()} type={getType()} isCustom={isCustom()} checksum={Checksum}";

		public string Name => GetObjName();

		public unsafe string GetObjName()
		{
			//return Encoding.ASCII.GetString(Name);
			fixed (byte* ptr = name)
			{
				return Encoding.ASCII.GetString(ptr, 8);
			}
		}

		private readonly byte SourceGame => (byte)(Flags >> 6 & 0x3);

		public readonly ObjectType ObjectType => (ObjectType)(Flags & 0x3F);

		private readonly bool IsCustom => SourceGame == 0;
	}
}
