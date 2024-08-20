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

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x38)]
	[LocoStructType(ObjectType.Competitor)]
	[LocoStringTable("Full Name", "Last Name")]
	public record CompetitorObject(
			[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id FullName,
			[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id LastName,
			[property: LocoStructOffset(0x04)] uint32_t var_04,
			[property: LocoStructOffset(0x08)] uint32_t var_08,
			[property: LocoStructOffset(0x0C)] uint32_t Emotions,
			[property: LocoStructOffset(0x10), Browsable(false), LocoArrayLength(CompetitorObject.ImagesLength)] image_id[] Images,
			[property: LocoStructOffset(0x34)] uint8_t Intelligence,
			[property: LocoStructOffset(0x35)] uint8_t Aggressiveness,
			[property: LocoStructOffset(0x36)] uint8_t Competitiveness,
			[property: LocoStructOffset(0x37)] uint8_t var_37
		) : ILocoStruct, ILocoImageTableNames
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
