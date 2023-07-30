using System.Runtime.InteropServices;
using System.Text;

namespace OpenLocoTool.Headers
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0x10)]
    public struct DatFileHeaderOld
    {
        public readonly uint flags;
        public readonly byte[] name = new byte[8] { 0xf, 0xf, 0xf, 0xf, 0xf, 0xf, 0xf, 0xf };
        public readonly uint checksum = 0xffffffff;

        public DatFileHeaderOld(ReadOnlySpan<byte> data)
        {
            flags = BitConverter.ToUInt32(data.Slice(0, 4));
            name = data.Slice(4, 8).ToArray();
            checksum = BitConverter.ToUInt32(data.Slice(12, 4));
        }

        public override string ToString() => $"filename=\"{Encoding.ASCII.GetString(name)}\" sourceGame={getSourceGame()} type={getType()} isCustom={isCustom()} checksum={checksum}";

        public string ObjName => Encoding.ASCII.GetString(name);

        private byte getSourceGame() => (byte)(flags >> 6 & 0x3);

        public ObjectType getType() => (ObjectType)(flags & 0x3F);

        private bool isCustom() => getSourceGame() == 0;

        //private bool isEmpty()
        //{
        //	auto ab = reinterpret_cast <const int64_t*> (this);
        //	return ab[0] == -1 && ab[1] == -1;
        //}
    }
}
