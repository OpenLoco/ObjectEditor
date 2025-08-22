using Dat.Converters;
using Dat.Types;
using Definitions.ObjectModels.Types;
using System.Text;

namespace Dat.FileParsing;

public class LocoBinaryWriter : BinaryWriter
{
	public LocoBinaryWriter(Stream output) : base(output, Encoding.UTF8, leaveOpen: true)
	{ }

	public void WriteBytes(int count = 1)
	{
		for (var i = 0; i < count; i++)
		{
			base.Write((uint8_t)0);
		}
	}

	public void WriteStringId(int count = 1)
	{
		for (var i = 0; i < count; i++)
		{
			base.Write((string_id)0);
		}
	}

	public void WriteImageId(int count = 1)
	{
		for (var i = 0; i < count; i++)
		{
			base.Write((image_id)0);
		}
	}

	public void WriteObjectId(int count = 1)
	{
		for (var i = 0; i < count; i++)
		{
			base.Write((object_id)0);
		}
	}

	public void WritePointer(int count = 1)
	{
		for (var i = 0; i < count; i++)
		{
			base.Write((uint32_t)0);
		}
	}

	public void WriteS5HeaderList(IEnumerable<S5Header> headers)
	{
		foreach (var header in headers)
		{
			Write(header.Write());
		}
	}

	public void WriteS5Header(ObjectModelHeader header)
	{
		var s5Header = new S5Header(header.Name, header.Checksum)
		{
			ObjectType = header.ObjectType.Convert(),
			ObjectSource = header.ObjectSource.Convert()
		};
		Write(s5Header.Write());
	}

	public void WriteS5HeaderList(IEnumerable<ObjectModelHeader> headers)
	{
		foreach (var header in headers)
		{
			WriteS5Header(header);
		}
	}
}
