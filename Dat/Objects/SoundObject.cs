using OpenLoco.Dat.Data;
using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Types;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Dat.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x1E)]
	public record SoundObjectData(
		[property: LocoStructOffset(0x00)] int32_t var_00,
		[property: LocoStructOffset(0x04)] int32_t Offset,
		[property: LocoStructOffset(0x08)] uint32_t Length,
		[property: LocoStructOffset(0x0C)] WaveFormatEx PcmHeader
		) : ILocoStruct
	{
		public SoundObjectData() : this(0, 0, 0, new WaveFormatEx())
		{ }

		public bool Validate()
			=> Offset >= 0;
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x12)]
	public record WaveFormatEx(
		[property: LocoStructOffset(0x00)] int16_t WaveFormatTag,
		[property: LocoStructOffset(0x02)] int16_t NumberOfChannels,
		[property: LocoStructOffset(0x04)] int32_t SampleRate,
		[property: LocoStructOffset(0x08)] int32_t AverageBytesPerSecond,
		[property: LocoStructOffset(0x0B)] int16_t BlockAlign,
		[property: LocoStructOffset(0x0D)] int16_t BitsPerSample,
		[property: LocoStructOffset(0x010)] int16_t CBSize
		) : ILocoStruct
	{
		public WaveFormatEx() : this(0, 0, 0, 0, 0, 0, 0)
		{ }

		public bool Validate()
			=> true;
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x0C)]
	[LocoStructType(ObjectType.Sound)]
	[LocoStringTable("Name")]
	public record SoundObject(
		[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
		[property: LocoStructOffset(0x02), Browsable(false)] uint32_t SoundObjectDataPtr,
		[property: LocoStructOffset(0x06)] uint8_t ShouldLoop, // 0 means no loop, any other number means loop (usually 1)
		[property: LocoStructOffset(0x07), Browsable(false)] uint8_t pad_07,
		[property: LocoStructOffset(0x08)] uint32_t Volume
		) : ILocoStruct, ILocoStructVariableData
	{
		[Editable(false)] public SoundObjectData SoundObjectData { get; set; }

		[Browsable(false)] public byte[] PcmData { get; set; } = [];

		uint numUnkStructs;
		uint pcmDataLength;
		byte[] unkData;

		public ReadOnlySpan<byte> LoadVariable(ReadOnlySpan<byte> remainingData)
		{
			// unknown structs
			numUnkStructs = BitConverter.ToUInt32(remainingData[0..4]);
			remainingData = remainingData[4..];

			// pcm data length
			pcmDataLength = BitConverter.ToUInt32(remainingData[0..4]); // unused
			remainingData = remainingData[4..];

			// unk
			unkData = remainingData[..(int)(numUnkStructs * 16)].ToArray();
			remainingData = remainingData[(int)(numUnkStructs * 16)..];

			// pcm data
			SoundObjectData = ByteReader.ReadLocoStruct<SoundObjectData>(remainingData[..ObjectAttributes.StructSize<SoundObjectData>()]);
			remainingData = remainingData[ObjectAttributes.StructSize<SoundObjectData>()..];

			PcmData = remainingData.ToArray();

			return remainingData[remainingData.Length..];
		}

		public ReadOnlySpan<byte> SaveVariable()
		{
			using (var ms = new MemoryStream())
			using (var br = new BinaryWriter(ms))
			{
				br.Write(numUnkStructs);
				br.Write(pcmDataLength);
				br.Write(unkData);
				br.Write(ByteWriter.WriteLocoStruct(SoundObjectData));
				br.Write(PcmData);

				br.Flush();
				ms.Flush();

				return ms.ToArray();
			}
		}

		public bool Validate() => true;
	}
}
