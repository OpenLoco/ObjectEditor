using Dat.Converters;
using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.Steam;
using Definitions.ObjectModels.Types;
using System.ComponentModel;
using System.IO;
using static Dat.Objects.SteamObjectLoader;

namespace Dat.Objects;

public abstract class SteamObjectLoader : IDatObjectLoader
{
	public static class Constants
	{
		public const int MaxSoundEffects = 9;
	}

	public static class Sizes
	{
		public const int ImageAndHeight = 2;
	}

	public static LocoObject Load(MemoryStream stream)
	{
		using (var br = new LocoBinaryReader(stream))
		{
			var model = new SteamObject();
			var stringTable = new StringTable();
			var imageTable = new List<GraphicsElement>();

			// fixed
			_ = br.SkipStringId(); // Name offset, not part of object definition
			_ = br.SkipUInt16(); // Count of images in graphics table
			model.NumStationaryTicks = br.ReadByte();
			_ = br.SkipByte(); // SpriteWidth
			_ = br.SkipByte(); // SpriteHeightNegative
			_ = br.SkipByte(); // SpriteHeightPositive
			model.Flags = ((DatSteamObjectFlags)br.ReadUInt16()).Convert();
			model.var_0A = br.ReadUInt32();
			_ = br.SkipImageId(); // BaseImageId, not used
			_ = br.SkipUInt16(); // _TotalNumFramesType0, not used
			_ = br.SkipUInt16(); // _TotalNumFramesType1, not used
			_ = br.SkipPointer(); // _FrameInfoType0Ptr, not used
			_ = br.SkipPointer(); // _FrameInfoType1Ptr, not used
			var numSoundEffects = br.ReadByte();
			_ = br.SkipObjectId(Constants.MaxSoundEffects);

			// string table
			stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType.Steam), null);

			// variable
			LoadVariable(br, model, numSoundEffects);

			// image table
			imageTable = SawyerStreamReader.ReadImageTableStream(stream).Table;

			return new LocoObject(ObjectType.Steam, model, stringTable, imageTable);
		}
	}

	private static void LoadVariable(LocoBinaryReader br, SteamObject model, byte numSoundEffects)
	{
		while (br.PeekByte() != 0xFF)
		{
			model.FrameInfoType0.Add(new ImageAndHeight() { ImageOffset = br.ReadByte(), Height = br.ReadByte() });
		}
		_ = br.SkipByte(); // skip the 0xFF terminator

		while (br.PeekByte() != 0xFF)
		{
			model.FrameInfoType1.Add(new ImageAndHeight() { ImageOffset = br.ReadByte(), Height = br.ReadByte() });
		}
		_ = br.SkipByte(); // skip the 0xFF terminator

		model.SoundEffects = SawyerStreamReader.LoadVariableCountS5HeadersStream(br, numSoundEffects);
	}

	public static void Save(MemoryStream stream, LocoObject obj)
	{
		var model = obj.Object as SteamObject;

		using (var bw = new LocoBinaryWriter(stream))
		{
			// fixed
			bw.WriteStringId(); // Name offset, not part of object definition
			bw.Write(model.NumStationaryTicks);
			bw.WriteByte(); // SpriteWidth, not used
			bw.WriteByte(); // SpriteHeightNegative, not used
			bw.WriteByte(); // SpriteHeightPositive, not used
			bw.Write((uint16_t)model.Flags.Convert());
			bw.Write(model.var_0A);
			bw.WriteImageId(); // BaseImageId, not used
			bw.WriteUint16(); // _TotalNumFramesType0, not used
			bw.WriteUint16(); // _TotalNumFramesType1, not used
			bw.WritePointer(); // _FrameInfoType0Ptr, not used
			bw.WritePointer(); // _FrameInfoType1Ptr, not used
			bw.Write((uint8_t)model.SoundEffects.Count);
			bw.WriteObjectId(Constants.MaxSoundEffects); // _SoundEffects, not used

			// string table
			SawyerStreamWriter.WriteStringTableStream(stream, obj.StringTable);

			// variable
			bw.Write(model.FrameInfoType0.SelectMany(x => new byte[] { x.ImageOffset, x.Height }).ToArray());
			bw.Write(0xFF); // end of frame info type 0
			bw.Write(model.FrameInfoType1.SelectMany(x => new byte[] { x.ImageOffset, x.Height }).ToArray());
			bw.Write(0xFF); // end of frame info type 1

			// image table
			SawyerStreamWriter.WriteImageTableStream(stream, obj.GraphicsElements);
		}
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

static class SteamObjectFlagsConverter
{
	public static SteamObjectFlags Convert(this DatSteamObjectFlags datSteamObjectFlags)
		=> (SteamObjectFlags)datSteamObjectFlags;

	public static DatSteamObjectFlags Convert(this SteamObjectFlags steamObjectFlags)
		=> (DatSteamObjectFlags)steamObjectFlags;
}

[LocoStructSize(0x28)]
[LocoStructType(DatObjectType.Steam)]
internal record DatSteamObject(
	[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
	[property: LocoStructOffset(0x02), LocoPropertyMaybeUnused] uint16_t NumImages, // this is simply the count of images in the graphics table
	[property: LocoStructOffset(0x04)] uint8_t NumStationaryTicks,
	[property: LocoStructOffset(0x05), LocoStructVariableLoad, Browsable(false)] uint8_t SpriteWidth,
	[property: LocoStructOffset(0x06), LocoStructVariableLoad, Browsable(false)] uint8_t SpriteHeightNegative,
	[property: LocoStructOffset(0x07), LocoStructVariableLoad, Browsable(false)] uint8_t SpriteHeightPositive,
	[property: LocoStructOffset(0x08)] DatSteamObjectFlags Flags,
	[property: LocoStructOffset(0x0A)] uint32_t var_0A,
	[property: LocoStructOffset(0x0E), Browsable(false)] image_id BaseImageId,
	[property: LocoStructOffset(0x12), LocoStructVariableLoad, Browsable(false)] uint16_t _TotalNumFramesType0,
	[property: LocoStructOffset(0x14), LocoStructVariableLoad, Browsable(false)] uint16_t _TotalNumFramesType1,
	[property: LocoStructOffset(0x16)] uint32_t _FrameInfoType0Ptr,
	[property: LocoStructOffset(0x1A)] uint32_t _FrameInfoType1Ptr,
	[property: LocoStructOffset(0x1E)] uint8_t NumSoundEffects,
	[property: LocoStructOffset(0x01F), LocoArrayLength(SteamObjectLoader.Constants.MaxSoundEffects), LocoStructVariableLoad, Browsable(false)] object_id[] _SoundEffects
)
{ }
