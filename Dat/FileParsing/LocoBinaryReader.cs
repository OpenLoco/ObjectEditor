using System.Text;

namespace Dat.FileParsing;

public class LocoBinaryReader : BinaryReader
{
	public LocoBinaryReader(Stream input) : base(input, Encoding.UTF8)
	{ }

	public LocoBinaryReader(Stream input, bool leaveOpen) : base(input, Encoding.UTF8, leaveOpen)
	{ }

	[Obsolete("This method does not correctly read bytes from the stream. Use PeekByte instead.")]
	public override int PeekChar()
		=> throw new NotImplementedException("This method does not correctly read bytes from the stream. Use PeekByte instead.");

	[Obsolete("Use as specific Read method for the data type you want to read.")]
	public override int Read()
		=> throw new NotImplementedException("Use as specific Read method for the data type you want to read.");

	public long SkipBytes(int count)
		=> BaseStream.Seek(count, SeekOrigin.Current);

	public long SkipStringId(int count = 1)
		=> BaseStream.Seek(2 * count, SeekOrigin.Current);

	public long SkipImageId(int count = 1)
		=> BaseStream.Seek(4 * count, SeekOrigin.Current);

	public long SkipPointer(int count = 1)
		=> BaseStream.Seek(4 * count, SeekOrigin.Current);

	public byte PeekByte()
	{
		var currentPosition = BaseStream.Position;
		var value = ReadByte();
		_ = BaseStream.Seek(currentPosition, SeekOrigin.Begin);
		return value;
	}

	public byte[] ReadToEnd()
	{
		var currentPosition = BaseStream.Position;
		var length = (int)(BaseStream.Length - currentPosition);
		var buffer = new byte[length];
		_ = Read(buffer, 0, length);
		return buffer;
	}
}
