using OpenLoco.Dat.Data;
using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Types;
using System.ComponentModel;

namespace OpenLoco.Dat.Objects
{
	public enum Emotion
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
	public enum CompetitorNamePrefix : uint32_t
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
	public enum CompetitorPlaystyle : uint32_t
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
	[LocoStructType(ObjectType.Competitor)]
	[LocoStringTable("Full Name", "Last Name")]
	public record CompetitorObject(
			[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id FullName,
			[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id LastName,
			[property: LocoStructOffset(0x04)] CompetitorNamePrefix AvailableNamePrefixes, // bitset
			[property: LocoStructOffset(0x08)] CompetitorPlaystyle AvailablePlaystyles, // bitset
			[property: LocoStructOffset(0x0C)] uint32_t Emotions,
			[property: LocoStructOffset(0x10), Browsable(false), LocoArrayLength(CompetitorObject.ImagesLength)] image_id[] Images,
			[property: LocoStructOffset(0x34)] uint8_t Intelligence,
			[property: LocoStructOffset(0x35)] uint8_t Aggressiveness,
			[property: LocoStructOffset(0x36)] uint8_t Competitiveness,
			[property: LocoStructOffset(0x37), LocoPropertyMaybeUnused] uint8_t var_37
		) : ILocoStruct, IImageTableNameProvider
	{
		public const int ImagesLength = 9;

		public bool Validate()
		{
			if ((Emotions & (1 << 0)) == 0)
			{
				return false;
			}

			if (Intelligence is < 1 or > 9)
			{
				return false;
			}

			if (Aggressiveness is < 1 or > 9)
			{
				return false;
			}

			return Competitiveness is >= 1 and <= 9;
		}

		public bool TryGetImageName(int id, out string? value)
			=> ImageIdNameMap.TryGetValue(id, out value);

		public static Dictionary<int, string> ImageIdNameMap = new()
		{
			{ 0, "smallNeutral" },
			{ 1, "largeNeutral" },
			{ 2, "smallHappy" },
			{ 3, "largeHappy" },
			{ 4, "smallWorried" },
			{ 5, "largeWorried" },
			{ 6, "smallThinking" },
			{ 7, "largeThinking" },
			{ 8, "smallDejected" },
			{ 9, "largeDejected" },
			{ 10, "smallSurprised" },
			{ 11, "largeSurprised" },
			{ 12, "smallScared" },
			{ 13, "largeScared" },
			{ 14, "smallAngry" },
			{ 15, "largeAngry" },
			{ 16, "smallDisgusted" },
			{ 17, "largeDisgusted" },
		};
	}
}
