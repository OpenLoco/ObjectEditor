using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Graphics;
using Definitions.ObjectModels.Objects.Wall;
using Definitions.ObjectModels.Types;
using static Dat.Loaders.WallObjectLoader;

namespace Dat.Loaders;

public abstract class WallObjectLoader : IDatObjectLoader
{
	public static class Constants
	{ }

	internal static class StructSizes
	{
		public const int Dat = 0x0A;
	}

	public static ObjectType ObjectType => ObjectType.Wall;
	public static DatObjectType DatObjectType => DatObjectType.Wall;

	public static LocoObject Load(Stream stream)
	{
		var initialStreamPosition = stream.Position;

		using (var br = new LocoBinaryReader(stream))
		{
			var model = new WallObject();

			// fixed
			br.SkipStringId(); // Name offset, not part of object definition
			br.SkipImageId(); // Image offset, not part of object definition
			model.ToolId = br.ReadByte(); // tool cursor type not used in Locomotion
			model.Flags1 = ((DatWallObjectFlags1)br.ReadByte()).Convert();
			model.Height = br.ReadByte();
			model.Flags2 = ((DatWallObjectFlags2)br.ReadByte()).Convert();

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + ObjectAttributes.StructSize(DatObjectType), nameof(stream.Position));

			// string table
			var stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType), null);

			// variable
			// N/A

			// image table
			var imageList = SawyerStreamReader.ReadImageTable(br).Table;

			// define groups
			var imageTable = ImageTableGrouper.CreateImageTable(model, ObjectType, imageList);

			return new LocoObject(ObjectType, model, stringTable, imageTable);
		}
	}

	public static void Save(Stream stream, LocoObject obj)
	{
		var initialStreamPosition = stream.Position;
		var model = (WallObject)obj.Object;

		using (var bw = new LocoBinaryWriter(stream))
		{
			bw.WriteEmptyStringId(); // Name offset, not part of object definition
			bw.WriteEmptyImageId(); // Image offset, not part of object definition
			bw.Write(model.ToolId);
			bw.Write((uint8_t)model.Flags1.Convert());
			bw.Write(model.Height);
			bw.Write((uint8_t)model.Flags2.Convert());

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + ObjectAttributes.StructSize(DatObjectType), nameof(stream.Position));

			// string table
			SawyerStreamWriter.WriteStringTable(stream, obj.StringTable);

			// variable
			// N/A

			// image table
			SawyerStreamWriter.WriteImageTable(stream, obj.ImageTable.GraphicsElements);
		}
	}

	[Flags]
	internal enum DatWallObjectFlags1 : uint8_t
	{
		None = 0,
		HasPrimaryColour = 1 << 0,
		HasGlass = 1 << 1,  // unused? only for rct2?
		OnlyOnLevelLand = 1 << 2,
		DoubleSided = 1 << 3, // unused? only for rct2?
		Door_UNUSED = 1 << 4, // unused, setting it in loco does nothing
		LongDoor_UNUSED = 1 << 5, // unused, setting it in loco does nothing
		HasSecondaryColour = 1 << 6,
		HasTertiaryColour_UNUSED = 1 << 7, // unused, setting it in loco does nothing
	}

	// this is copied from OpenRCT2: https://github.com/OpenRCT2/OpenRCT2/blob/develop/src/openrct2/object/WallSceneryEntry.h
	[Flags]
	internal enum DatWallObjectFlags2 : uint8_t
	{
		None = 0,
		NoSelectPrimaryColour = 1 << 0,
		DoorSoundMask = 1 << 1,  // unused? only for rct2?
		DoorSoundShift = 1 << 2, // unused? only for rct2?
		Opaque = 1 << 3, // unused? only for rct2?
		Animated = 1 << 4, // unused? only for rct2?
	}
}

internal static class WallObjectFlags1Converter
{
	public static WallObjectFlags1 Convert(this DatWallObjectFlags1 datWallObjectFlags1)
		=> (WallObjectFlags1)datWallObjectFlags1;

	public static DatWallObjectFlags1 Convert(this WallObjectFlags1 wallObjectFlags1)
		=> (DatWallObjectFlags1)wallObjectFlags1;
}

internal static class WallObjectFlags2Converter
{
	public static WallObjectFlags2 Convert(this DatWallObjectFlags2 datWallObjectFlags2)
		=> (WallObjectFlags2)datWallObjectFlags2;

	public static DatWallObjectFlags2 Convert(this WallObjectFlags2 wallObjectFlags2)
		=> (DatWallObjectFlags2)wallObjectFlags2;
}
