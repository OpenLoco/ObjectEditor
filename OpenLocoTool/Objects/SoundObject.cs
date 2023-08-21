using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x0C)]
	public record SoundObject(
		[property: LocoStructOffset(0x00)] string_id Name,
		//[property: LocoStructProperty(0x02)] const SoundObjectData* Data,
		[property: LocoStructOffset(0x06)] uint8_t var_06,
		[property: LocoStructOffset(0x07)] uint8_t pad_07,
		[property: LocoStructOffset(0x08)] uint32_t Volume
		) : ILocoStruct, ILocoStructExtraLoading
	{
		public static ObjectType ObjectType => ObjectType.sound;
		public static int StructSize => 0x0C;

		// create struct SoundObjectData, see SoundObject.h
		byte[] pcmData;

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
			pcmData = remainingData[..(int)pcmDataLength].ToArray();
			remainingData = remainingData[(int)pcmDataLength..];

			return remainingData;
		}
	}
}
