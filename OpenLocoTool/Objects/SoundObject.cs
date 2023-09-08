using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x12)]
	[LocoStringCount(0)]
	public record WaveFormatEx(
		[property: LocoStructOffset(0x00)] int16_t WaveFormatTag,
		[property: LocoStructOffset(0x02)] int16_t NumberChannels,
		[property: LocoStructOffset(0x04)] int32_t SamplesPerSecond,
		[property: LocoStructOffset(0x08)] int32_t AverageBytesPerSecond,
		[property: LocoStructOffset(0x0B)] int16_t BlockAlign,
		[property: LocoStructOffset(0x0D)] int16_t BitsPerSample,
		[property: LocoStructOffset(0x010)] int16_t CBSize
		) : ILocoStruct
	{
		public static int StructSize => 0x12;
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x1E)]
	[LocoStringCount(0)]
	public record SoundObjectData(
		[property: LocoStructOffset(0x00)] int32_t var_00,
		[property: LocoStructOffset(0x04)] int32_t Offset,
		[property: LocoStructOffset(0x08)] uint32_t Length,
		[property: LocoStructOffset(0x0C)] WaveFormatEx PcmHeader
		) : ILocoStruct
	{
		public static int StructSize => 0x1E;
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x0C)]
	[LocoStringCount(1)]
	public record SoundObject(
		[property: LocoStructOffset(0x00)] string_id Name,
		[property: LocoStructOffset(0x02)] uint32_t SoundObjectDataPtr,
		[property: LocoStructOffset(0x06)] uint8_t var_06,
		[property: LocoStructOffset(0x07)] uint8_t pad_07,
		[property: LocoStructOffset(0x08)] uint32_t Volume
		) : ILocoStruct, ILocoStructVariableData
	{
		public static ObjectType ObjectType => ObjectType.Sound;
		public static int StructSize => 0x0C;

		public SoundObjectData SoundObjectData { get; set; }

		public byte[] RawPcmData { get; set; }

		public ReadOnlySpan<byte> Load(ReadOnlySpan<byte> remainingData)
		{
			// unknown structs
			var numUnkStructs = BitConverter.ToUInt32(remainingData[0..4]);
			remainingData = remainingData[4..];

			// pcm data length
			var pcmDataLength = BitConverter.ToUInt32(remainingData[0..4]);
			remainingData = remainingData[4..];

			remainingData = remainingData[(int)(numUnkStructs * 16)..];

			// pcm data
			SoundObjectData = (SoundObjectData)ByteReader.ReadLocoStruct<SoundObjectData>(remainingData[..SoundObjectData.StructSize]);
			remainingData = remainingData[SoundObjectData.StructSize..];

			RawPcmData = remainingData.ToArray();

			remainingData = remainingData[remainingData.Length..];

			return remainingData;
		}

		public ReadOnlySpan<byte> Save() => throw new NotImplementedException();
	}
}
