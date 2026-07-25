// DAT/S5 binary parsing — nullable analysis cannot reason about offset-based field population.
#pragma warning disable CS8618, CS8602, CS8604, CS8601, CS8625, CS8629

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

	public static ObjectType ObjectType => ObjectType.Sound;
	public static DatObjectType DatObjectType => DatObjectType.Sound;

	public static LocoObject Load(Stream stream)
	{
		var initialStreamPosition = stream.Position;

		using (var br = new LocoBinaryReader(stream))
		{
			var model = new SoundObject();

			// fixed
			br.SkipStringId(); // Name offset, not part of object definition
			br.SkipPointer(); // SoundObjectDataPtr, not part of object definition
			model.ShouldLoop = br.ReadByte();
			br.SkipByte(); // 0x07 is a padding byte
			model.Volume = br.ReadUInt32();

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + ObjectAttributes.StructSize(DatObjectType), nameof(stream.Position));

			// string table
			var stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType), null);

			// variable
			LoadVariable(br, model);

			// image table
			// N/A

			return new LocoObject(ObjectType, model, stringTable);
		}
	}

	private static void LoadVariable(LocoBinaryReader br, SoundObject model)
	{
		model.NumUnkStructs = br.ReadUInt32();
		_ = br.ReadUInt32(); // unused
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

	public static void Save(Stream stream, LocoObject obj)
	{
		var initialStreamPosition = stream.Position;
		var model = (SoundObject)obj.Object;

		using (var bw = new LocoBinaryWriter(stream))
		{
			bw.WriteEmptyStringId(); // Name offset, not part of object definition
			bw.WriteEmptyPointer();
			bw.Write(model.ShouldLoop);
			bw.WriteEmptyBytes(1); // 0x07 is a padding byte
			bw.Write(model.Volume);

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + ObjectAttributes.StructSize(DatObjectType), nameof(stream.Position));

			// string table
			SawyerStreamWriter.WriteStringTable(stream, obj.StringTable);

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
