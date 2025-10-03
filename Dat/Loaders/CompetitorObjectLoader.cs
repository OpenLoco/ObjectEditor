using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Graphics;
using Definitions.ObjectModels.Objects.Competitor;
using Definitions.ObjectModels.Types;

namespace Dat.Loaders;

public abstract class CompetitorObjectLoader : IDatObjectLoader
{
	public static class Constants
	{
		public const int ImagesLength = 9;
	}

	public static ObjectType ObjectType => ObjectType.Competitor;
	public static DatObjectType DatObjectType => DatObjectType.Competitor;

	public static LocoObject Load(Stream stream)
	{
		var initialStreamPosition = stream.Position;

		using (var br = new LocoBinaryReader(stream))
		{
			var model = new CompetitorObject();

			// fixed
			br.SkipStringId(); // First name offset, not part of object definition
			br.SkipStringId(); // Last name offset, not part of object definition
			model.AvailableNamePrefixes = (NamePrefixFlags)br.ReadUInt32();
			model.AvailablePlayStyles = (PlaystyleFlags)br.ReadUInt32();
			model.Emotions = (EmotionFlags)br.ReadUInt32();
			br.SkipImageId(Constants.ImagesLength); // Images, not part of object definition
			model.Intelligence = br.ReadByte();
			model.Aggressiveness = br.ReadByte();
			model.Competitiveness = br.ReadByte();
			model.var_37 = br.ReadByte(); // unused

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
		var model = (CompetitorObject)obj.Object;

		using (var bw = new LocoBinaryWriter(stream))
		{
			bw.WriteEmptyStringId(); // First ame offset, not part of object definition
			bw.WriteEmptyStringId(); // Last name offset, not part of object definition
			bw.Write((uint32_t)model.AvailableNamePrefixes);
			bw.Write((uint32_t)model.AvailablePlayStyles);
			bw.Write((uint32_t)model.Emotions);
			bw.WriteEmptyImageId(Constants.ImagesLength); // Images, not part of object definition
			bw.Write(model.Intelligence);
			bw.Write(model.Aggressiveness);
			bw.Write(model.Competitiveness);
			bw.Write(model.var_37); // unused

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
	internal enum DatCompetitorEmotion : uint32_t
	{
		Neutral = 1 << 0,
		Happy = 1 << 1,
		Worried = 1 << 2,
		Thinking = 1 << 3,
		Dejected = 1 << 4,
		Surprised = 1 << 5,
		Scared = 1 << 6,
		Angry = 1 << 7,
		Disgusted = 1 << 8,
	}

	[Flags]
	internal enum DatCompetitorNamePrefix : uint32_t
	{
		unk0 = 1 << 0,
		unk1 = 1 << 1,
		unk2 = 1 << 2,
		unk3 = 1 << 3,
		unk4 = 1 << 4,
		unk5 = 1 << 5,
		unk6 = 1 << 6,
		unk7 = 1 << 7,
		unk8 = 1 << 8,
		unk9 = 1 << 9,
		unk10 = 1 << 10,
		unk11 = 1 << 11,
		unk12 = 1 << 12,
	}

	[Flags]
	internal enum DatCompetitorPlaystyle : uint32_t
	{
		unk0 = 1 << 0,
		unk1 = 1 << 1,
		unk2 = 1 << 2,
		unk3 = 1 << 3,
		unk4 = 1 << 4,
		unk5 = 1 << 5,
		unk6 = 1 << 6,
		unk7 = 1 << 7,
		unk8 = 1 << 8,
		unk9 = 1 << 9,
		unk10 = 1 << 10,
		unk11 = 1 << 11,
		unk12 = 1 << 12,
	}
}
