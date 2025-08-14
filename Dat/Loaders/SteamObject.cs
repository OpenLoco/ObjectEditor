using Dat.Converters;
using Dat.Data;
using Dat.FileParsing;
using Dat.Types;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.Steam;
using Definitions.ObjectModels.Types;
using System.ComponentModel;
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
		using (var br = new BinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true))
		{
			var model = new SteamObject();
			var stringTable = new StringTable();
			var imageTable = new List<GraphicsElement>();

			// fixed
			_ = br.BaseStream.Seek(0x02, SeekOrigin.Begin); // Skip name offset
			model.NumStationaryTicks = br.ReadByte();
			_ = br.BaseStream.Seek(0x08, SeekOrigin.Begin); // Skip image data that is loaded in variable load
			model.Flags = ((DatSteamObjectFlags)br.ReadUInt16()).Convert();
			model.var_0A = br.ReadUInt32();
			_ = br.BaseStream.Seek(0x1E, SeekOrigin.Begin); // Move variable load stuff
			var numSoundEffects = br.ReadByte();

			// move to string table
			var structSize = ObjectAttributes.StructSize(DatObjectType.Steam);
			_ = br.BaseStream.Seek(structSize, SeekOrigin.Begin);

			// string table
			stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType.Steam), null);

			// variable
			//model.CompatibleTrackObjects = [.. SawyerStreamReader
			//	.LoadVariableCountS5HeadersStream(stream, compatibleTrackCount)
			//	.Select(x => new ObjectModelHeader(x.Name, x.Checksum, x.ObjectType.Convert(), x.ObjectSource.Convert()))];

			// image table
			imageTable = SawyerStreamReader.ReadImageTableStream(stream).Table;

			return new LocoObject(ObjectType.Steam, model, stringTable, imageTable);
		}
	}

	public static void Save(MemoryStream ms, LocoObject obj)
	{
		var model = obj.Object as SteamObject;

		using (var bw = new LocoBinaryWriter(ms, System.Text.Encoding.UTF8, leaveOpen: true))
		{
			// fixed
			bw.Write((string_id)0);
			bw.Write(model.NumStationaryTicks);
			bw.Write(0); // SpriteWidth, not used
			bw.Write(0); // SpriteHeightNegative, not used
			bw.Write(0); // SpriteHeightPositive, not used
			bw.Write((uint16_t)model.Flags.Convert());
			bw.Write(model.var_0A);
			bw.Write((image_id)0); // BaseImageId, not used
			bw.Write((uint16_t)0); // _TotalNumFramesType0, not used
			bw.Write((uint16_t)0); // _TotalNumFramesType1, not used
			bw.Write(0); // _FrameInfoType0Ptr, not used
			bw.Write(0); // _FrameInfoType1Ptr, not used
			bw.Write((uint8_t)model.SoundEffects.Count);
			bw.WriteRepeated((object_id)0, Constants.MaxSoundEffects); // _SoundEffects, not used

			// string table

			// variable
			bw.Write(model.FrameInfoType0.SelectMany(x => new byte[] { x.ImageOffset, x.Height }).ToArray());
			bw.Write(0xFF); // end of frame info type 0
			bw.Write(model.FrameInfoType1.SelectMany(x => new byte[] { x.ImageOffset, x.Height }).ToArray());
			bw.Write(0xFF); // end of frame info type 1
			bw.WriteS5HeaderList(model.SoundEffects);

			// image table
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
		=> datSteamObjectFlags switch
		{
			DatSteamObjectFlags.None => SteamObjectFlags.None,
			DatSteamObjectFlags.ApplyWind => SteamObjectFlags.ApplyWind,
			DatSteamObjectFlags.DisperseOnCollision => SteamObjectFlags.DisperseOnCollision,
			DatSteamObjectFlags.unk_02 => SteamObjectFlags.unk_02,
			DatSteamObjectFlags.unk_03 => SteamObjectFlags.unk_03,
			_ => throw new ArgumentOutOfRangeException(nameof(datSteamObjectFlags), datSteamObjectFlags, null)
		};

	public static DatSteamObjectFlags Convert(this SteamObjectFlags steamObjectFlags)
		=> steamObjectFlags switch
		{
			SteamObjectFlags.None => DatSteamObjectFlags.None,
			SteamObjectFlags.ApplyWind => DatSteamObjectFlags.ApplyWind,
			SteamObjectFlags.DisperseOnCollision => DatSteamObjectFlags.DisperseOnCollision,
			SteamObjectFlags.unk_02 => DatSteamObjectFlags.unk_02,
			SteamObjectFlags.unk_03 => DatSteamObjectFlags.unk_03,
			_ => throw new ArgumentOutOfRangeException(nameof(steamObjectFlags), steamObjectFlags, null)
		};
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
