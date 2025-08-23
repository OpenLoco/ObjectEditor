using Common;
using Dat.Converters;
using Dat.Types;
using Definitions.ObjectModels.Objects.Airport;
using Definitions.ObjectModels.Objects.Sound;
using Definitions.ObjectModels.Types;
using System.Text;

namespace Dat.FileParsing;

public class LocoBinaryWriter : BinaryWriter
{
	public LocoBinaryWriter(Stream output) : base(output, Encoding.UTF8, leaveOpen: true)
	{ }

	public void WriteTerminator()
		=> base.Write(LocoConstants.Terminator);

	public void WriteBytes(int count)
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

	public void WriteS5Header(S5Header header)
		=> Write(header.Write());

	public void WriteS5Header(ObjectModelHeader? header)
		=> WriteS5Header(header?.Convert() ?? S5Header.NullHeader);

	public void WriteS5HeaderList(IEnumerable<ObjectModelHeader> headers, int minimum = 0)
	{
		foreach (var header in headers
			.Select(x => x.Convert())
			.Fill(minimum, S5Header.NullHeader))
		{
			WriteS5Header(header);
		}
	}

	public void WriteBuildingHeights(List<uint8_t> heights)
		=> Write([.. heights]);

	public void WriteBuildingAnimations(List<BuildingPartAnimation> animations)
	{
		foreach (var x in animations)
		{
			Write(x.NumFrames);
			Write(x.AnimationSpeed);
		}
	}

	public void WriteBuildingVariations(List<List<uint8_t>> variations)
	{
		foreach (var x in variations)
		{
			Write([.. x]);
			WriteTerminator();
		}
	}

	public void WriteSoundEffect(SoundEffectWaveFormat sfx)
	{
		Write(sfx.WaveFormatTag);
		Write(sfx.Channels);
		Write(sfx.SampleRate);
		Write(sfx.AverageBytesPerSecond);
		Write(sfx.BlockAlign);
		Write(sfx.BitsPerSample);
		Write(sfx.ExtraSize);
	}
}
