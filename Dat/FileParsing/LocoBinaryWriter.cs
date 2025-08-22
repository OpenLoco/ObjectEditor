using Dat.Converters;
using Dat.Types;
using Definitions.ObjectModels.Types;
using System.Text;

namespace Dat.FileParsing;

public class LocoBinaryWriter : BinaryWriter
{
	public LocoBinaryWriter(Stream output) : base(output, Encoding.UTF8, leaveOpen: true)
	{ }

	public void WriteByte(uint8_t value = 0)
		=> Write(value);

	public void WriteUInt16(uint16_t value = 0)
		=> Write(value);

	public void WriteInt16(int16_t value = 0)
		=> Write(value);

	public void WriteStringId()
		=> Write((string_id)0);

	public void WriteImageId()
		=> Write((image_id)0);

	public void WriteObjectId(int count = 1)
	{
		for (var i = 0; i < count; i++)
		{
			Write((object_id)0);
		}
	}

	public void WritePointer(int count = 1)
	{
		for (var i = 0; i < count; i++)
		{
			Write((uint32_t)0);
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
