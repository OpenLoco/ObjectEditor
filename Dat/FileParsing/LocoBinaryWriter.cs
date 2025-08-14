using Dat.Converters;
using Dat.Types;
using Definitions.ObjectModels.Types;
using System.Text;

namespace Dat.FileParsing;
public class LocoBinaryReader : BinaryReader
{
	public LocoBinaryReader(Stream input) : base(input)
	{ }

	public LocoBinaryReader(Stream input, Encoding encoding) : base(input, encoding)
	{ }

	public LocoBinaryReader(Stream input, Encoding encoding, bool leaveOpen) : base(input, encoding, leaveOpen)
	{ }

	public LocoBinaryReader(Stream input, bool leaveOpen) : base(input, Encoding.UTF8, leaveOpen)
	{ }

	public override int PeekChar()
		=> throw new NotImplementedException("Use PeekByte instead");

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

public class LocoBinaryWriter : BinaryWriter
{
	public LocoBinaryWriter(Stream output) : base(output, Encoding.UTF8)
	{ }

	public LocoBinaryWriter(Stream output, Encoding encoding) : base(output, encoding)
	{ }

	public LocoBinaryWriter(Stream output, Encoding encoding, bool leaveOpen) : base(output, encoding, leaveOpen)
	{ }

	public LocoBinaryWriter(Stream output, bool leaveOpen) : base(output, Encoding.UTF8, leaveOpen)
	{ }

	public void WriteRepeated(byte value, int repeatCount)
	{
		for (var i = 0; i < repeatCount; i++)
		{
			Write(value);
		}
	}

	public void WriteS5HeaderList(IEnumerable<S5Header> headers)
	{
		foreach (var header in headers)
		{
			Write(header.Write());
		}
	}

	public void WriteS5HeaderList(IEnumerable<ObjectModelHeader> headers)
	{
		foreach (var x in headers)
		{
			var s5Header = new S5Header(x.Name, x.Checksum)
			{
				ObjectType = x.ObjectType.Convert(),
				ObjectSource = x.ObjectSource.Convert()
			};
			Write(s5Header.Write());
		}
	}
}
