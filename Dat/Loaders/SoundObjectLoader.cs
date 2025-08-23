using Dat.Data;
using Dat.FileParsing;
using Dat.Types;
using Dat.Types.Audio;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.Sound;
using Definitions.ObjectModels.Types;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Dat.Loaders;

public abstract class SoundObjectLoader : IDatObjectLoader
{
	public static class Constants
	{
		public const int NumUnkStructs = 16;
	}

	public static class StructSizes
	{
		public const int Dat = 0x0C;
		public const int SoundObjectData = 0x1E;
	}

	public static LocoObject Load(MemoryStream stream)
	{
		var initialStreamPosition = stream.Position;

		using (var br = new LocoBinaryReader(stream))
		{
			var model = new SoundObject();
			var stringTable = new StringTable();
			var imageTable = new List<GraphicsElement>();

			// fixed
			br.SkipStringId(); // Name offset, not part of object definition
			br.SkipPointer(); // SoundObjectDataPtr, not part of object definition
			model.ShouldLoop = br.ReadByte();
			br.SkipByte(); // pad_07, not part of object definition
			model.Volume = br.ReadUInt32();

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + StructSizes.Dat, nameof(stream.Position));

			// string table
			stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType.Sound), null);

			// variable
			LoadVariable(br, model);

			// image table
			// N/A

			return new LocoObject(ObjectType.Sound, model, stringTable, imageTable);
		}
	}

	private static void LoadVariable(LocoBinaryReader br, SoundObject model)
	{
		model.NumUnkStructs = br.ReadUInt32();
		var pcmDataLength = br.ReadUInt32(); // unused
		model.UnkData = br.ReadBytes((int)model.NumUnkStructs * Constants.NumUnkStructs);
		model.SoundObjectData = new SoundObjectData
		{
			var_00 = br.ReadInt32(),
			Offset = br.ReadInt32(),
			Length = br.ReadUInt32(),
			PcmHeader = br.ReadSoundEffect(),
		};

		model.PcmData = br.ReadToEnd();
	}

	public static void Save(MemoryStream stream, LocoObject obj)
	{
		var initialStreamPosition = stream.Position;
		var model = (SoundObject)obj.Object;

		using (var bw = new LocoBinaryWriter(stream))
		{
			bw.WriteStringId(); // Name offset, not part of object definition
			bw.WritePointer();
			bw.Write(model.ShouldLoop);
			bw.WriteBytes(1);
			bw.Write(model.Volume);

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + StructSizes.Dat, nameof(stream.Position));

			// string table
			SawyerStreamWriter.WriteStringTableStream(stream, obj.StringTable);

			// variable
			SaveVariable(model, bw);

			// image table
			// N/A
		}
	}

	private static void SaveVariable(SoundObject model, LocoBinaryWriter bw)
	{
		bw.Write(model.NumUnkStructs);
		bw.Write((uint32_t)0); // unused pcm data length
		bw.Write(model.UnkData);

		var m = model.SoundObjectData;
		bw.Write(m.var_00);
		bw.Write(m.Offset);
		bw.Write(m.Length);
		bw.Write(m.PcmHeader.WaveFormatTag);
		bw.Write(m.PcmHeader.Channels);
		bw.Write(m.PcmHeader.SampleRate);
		bw.Write(m.PcmHeader.AverageBytesPerSecond);
		bw.Write(m.PcmHeader.BlockAlign);
		bw.Write(m.PcmHeader.BitsPerSample);
		bw.Write(m.PcmHeader.ExtraSize);

		bw.Write(model.PcmData);
	}
}

[LocoStructSize(0x1E)]
internal record DatSoundObjectData(
	[property: LocoStructOffset(0x00)] int32_t var_00,
	[property: LocoStructOffset(0x04)] int32_t Offset,
	[property: LocoStructOffset(0x08)] uint32_t Length,
	[property: LocoStructOffset(0x0C)] DatSoundEffectWaveFormat PcmHeader
	) : ILocoStruct
{
	public DatSoundObjectData() : this(0, 0, 0, new DatSoundEffectWaveFormat())
	{ }

	public bool Validate()
		=> Offset >= 0;
}

[LocoStructSize(0x0C)]
[LocoStructType(DatObjectType.Sound)]
internal record DatSoundObject(
	[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
	[property: LocoStructOffset(0x02), Browsable(false)] uint32_t SoundObjectDataPtr,
	[property: LocoStructOffset(0x06)] uint8_t ShouldLoop, // 0 means no loop, any other number means loop (usually 1)
	[property: LocoStructOffset(0x07), Browsable(false)] uint8_t pad_07,
	[property: LocoStructOffset(0x08)] uint32_t Volume
	) : ILocoStruct, ILocoStructVariableData
{
	[Editable(false)] public DatSoundObjectData SoundObjectData { get; set; }

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
		SoundObjectData = ByteReader.ReadLocoStruct<DatSoundObjectData>(remainingData[..ObjectAttributes.StructSize<DatSoundObjectData>()]);
		remainingData = remainingData[ObjectAttributes.StructSize<DatSoundObjectData>()..];

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
