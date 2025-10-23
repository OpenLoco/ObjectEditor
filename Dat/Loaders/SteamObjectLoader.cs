using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Graphics;
using Definitions.ObjectModels.Objects.Steam;
using Definitions.ObjectModels.Types;

using static Dat.Loaders.SteamObjectLoader;

namespace Dat.Loaders;

public abstract class SteamObjectLoader : IDatObjectLoader
{
	public static class Constants
	{
		public const int MaxSoundEffects = 9;
	}

	public static class StructSizes
	{
		public const int Dat = 0x28;
		public const int ImageAndHeight = 2;
	}

	public static ObjectType ObjectType => ObjectType.Steam;
	public static DatObjectType DatObjectType => DatObjectType.Steam;

	public static LocoObject Load(Stream stream)
	{
		var initialStreamPosition = stream.Position;

		using (var br = new LocoBinaryReader(stream))
		{
			var model = new SteamObject();

			// fixed
			br.SkipStringId(); // Name offset, not part of object definition
			br.SkipUInt16(); // Count of images in graphics table
			model.NumStationaryTicks = br.ReadByte();
			br.SkipByte(); // SpriteWidth
			br.SkipByte(); // SpriteHeightNegative
			br.SkipByte(); // SpriteHeightPositive
			model.Flags = ((DatSteamObjectFlags)br.ReadUInt16()).Convert();
			model.var_0A = br.ReadUInt32();
			br.SkipImageId(); // BaseImageId, not used
			br.SkipUInt16(); // _TotalNumFramesType0, not used
			br.SkipUInt16(); // _TotalNumFramesType1, not used
			br.SkipPointer(); // _FrameInfoType0Ptr, not used
			br.SkipPointer(); // _FrameInfoType1Ptr, not used
			var numSoundEffects = br.ReadByte();
			br.SkipObjectId(Constants.MaxSoundEffects);

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + ObjectAttributes.StructSize(DatObjectType), nameof(stream.Position));

			// string table
			var stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType), null);

			// variable
			LoadVariable(br, model, numSoundEffects);

			// image table
			var imageList = SawyerStreamReader.ReadImageTable(br).Table;

			// define groups
			var imageTable = ImageTableGrouper.CreateImageTable(model, ObjectType, imageList);

			return new LocoObject(ObjectType, model, stringTable, imageTable);
		}
	}

	private static void LoadVariable(LocoBinaryReader br, SteamObject model, byte numSoundEffects)
	{
		while (br.PeekByte() != LocoConstants.Terminator)
		{
			model.FrameInfoType0.Add(new() { ImageOffset = br.ReadByte(), Height = br.ReadByte() });
		}

		br.SkipTerminator();

		while (br.PeekByte() != LocoConstants.Terminator)
		{
			model.FrameInfoType1.Add(new() { ImageOffset = br.ReadByte(), Height = br.ReadByte() });
		}

		br.SkipTerminator();

		model.SoundEffects = [.. br.ReadS5HeaderList(numSoundEffects)];
	}

	public static void Save(Stream stream, LocoObject obj)
	{
		var initialStreamPosition = stream.Position;
		var model = obj.Object as SteamObject;

		using (var bw = new LocoBinaryWriter(stream))
		{
			// fixed
			bw.WriteEmptyStringId(); // Name offset, not part of object definition
			bw.Write((uint16_t)0);
			bw.Write(model.NumStationaryTicks);
			bw.Write((uint8_t)0); // SpriteWidth, not used
			bw.Write((uint8_t)0); // SpriteHeightNegative, not used
			bw.Write((uint8_t)0); // SpriteHeightPositive, not used
			bw.Write((uint16_t)model.Flags.Convert());
			bw.Write(model.var_0A);
			bw.WriteEmptyImageId(); // BaseImageId, not used
			bw.Write((uint16_t)0); // _TotalNumFramesType0, not used
			bw.Write((uint16_t)0); // _TotalNumFramesType1, not used
			bw.WriteEmptyPointer(); // _FrameInfoType0Ptr, not used
			bw.WriteEmptyPointer(); // _FrameInfoType1Ptr, not used
			bw.Write((uint8_t)model.SoundEffects.Count);
			bw.WriteEmptyObjectId(Constants.MaxSoundEffects); // _SoundEffects, not used

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + ObjectAttributes.StructSize(DatObjectType), nameof(stream.Position));

			// string table
			SawyerStreamWriter.WriteStringTable(stream, obj.StringTable);

			// variable
			SaveVariable(model, bw);

			// image table
			SawyerStreamWriter.WriteImageTable(stream, obj.ImageTable.GraphicsElements);
		}
	}

	private static void SaveVariable(SteamObject model, LocoBinaryWriter bw)
	{
		foreach (var fit in model.FrameInfoType0)
		{
			bw.Write(fit.ImageOffset);
			bw.Write(fit.Height);
		}

		bw.WriteTerminator(); // end of frame info type 0

		foreach (var fit in model.FrameInfoType1)
		{
			bw.Write(fit.ImageOffset);
			bw.Write(fit.Height);
		}

		bw.WriteTerminator(); // end of frame info type 1

		bw.WriteS5HeaderList(model.SoundEffects);
	}

	[Flags]
	internal enum DatSteamObjectFlags : uint16_t
	{
		None = 0,
		ApplyWind = 1 << 0,
		DisperseOnCollision = 1 << 1,
		unk_02 = 1 << 2,
		unk_03 = 1 << 3,
	}
}

internal static class SteamObjectFlagsConverter
{
	public static SteamObjectFlags Convert(this DatSteamObjectFlags datSteamObjectFlags)
		=> (SteamObjectFlags)datSteamObjectFlags;

	public static DatSteamObjectFlags Convert(this SteamObjectFlags steamObjectFlags)
		=> (DatSteamObjectFlags)steamObjectFlags;
}
