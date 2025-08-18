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

	public void WriteUint16(uint16_t value = 0)
		=> Write(value);

	public void WriteStringId(string_id value = 0)
		=> Write(value);

	public void WriteImageId(image_id value = 0)
		=> Write(value);

	public void WriteObjectId(object_id value = 0, int count = 1)
	{
		for (var i = 0; i < count; i++)
		{
			Write(value);
		}
	}

	public void WritePointer(uint32_t value = 0, int count = 1)
	{
		for (var i = 0; i < count; i++)
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
