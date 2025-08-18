using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using System.ComponentModel;

namespace Dat.Objects;

public abstract class CompetitorObjectLoader : IDatObjectLoader
{
	public static class Constants
	{
		public const int ImagesLength = 9;
	}

	public static class Sizes
	{ }

	public static LocoObject Load(MemoryStream stream) => throw new NotImplementedException();
	public static void Save(MemoryStream ms, LocoObject obj) => throw new NotImplementedException();
}

internal enum DatEmotion
{
	Neutral,
	Happy,
	Worried,
	Thinking,
	Dejected,
	Surprised,
	Scared,
	Angry,
	Disgusted,
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

[TypeConverter(typeof(ExpandableObjectConverter))]
[LocoStructSize(0x38)]
[LocoStructType(DatObjectType.Competitor)]
internal record DatCompetitorObject(
		[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id FullName,
		[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id LastName,
		[property: LocoStructOffset(0x04)] DatCompetitorNamePrefix AvailableNamePrefixes, // bitset
		[property: LocoStructOffset(0x08)] DatCompetitorPlaystyle AvailablePlaystyles, // bitset
		[property: LocoStructOffset(0x0C)] uint32_t Emotions, // bitset
		[property: LocoStructOffset(0x10), Browsable(false), LocoArrayLength(CompetitorObjectLoader.Constants.ImagesLength)] image_id[] Images,
		[property: LocoStructOffset(0x34)] uint8_t Intelligence,
		[property: LocoStructOffset(0x35)] uint8_t Aggressiveness,
		[property: LocoStructOffset(0x36)] uint8_t Competitiveness,
		[property: LocoStructOffset(0x37), LocoPropertyMaybeUnused] uint8_t var_37
	)
{ }
