using OpenLoco.Dat.Data;
using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Types;
using System.ComponentModel;

namespace OpenLoco.Dat.Objects.Sound
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x0C)]
	[LocoStructType(ObjectType.Sound)]
	[LocoStringTable("Name")]
	public record SoundObject(
		[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
		[property: LocoStructOffset(0x02), Browsable(false)] uint32_t SoundObjectDataPtr,
		[property: LocoStructOffset(0x06)] uint8_t LoopCount, // 0 means no loop, any other number means loop
		[property: LocoStructOffset(0x07)] uint8_t var_07,
		[property: LocoStructOffset(0x08)] uint32_t Volume
		) : ILocoStruct, ILocoStructVariableData
	{
		public SoundObjectData SoundObjectData { get; set; }

		public byte[] PcmData { get; set; } = [];

		uint numUnkStructs;
		uint pcmDataLength;
		byte[] unkData;

		public ReadOnlySpan<byte> Load(ReadOnlySpan<byte> remainingData)
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

		public ReadOnlySpan<byte> Save()
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
