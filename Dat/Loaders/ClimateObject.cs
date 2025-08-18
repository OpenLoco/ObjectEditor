using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using System.ComponentModel;

namespace Dat.Objects;

public abstract class ClimateObjectLoader : IDatObjectLoader
{
	public static class Constants
	{
		public const int Seasons = 4;
	}

	public static class Sizes
	{ }

	public static LocoObject Load(MemoryStream stream) => throw new NotImplementedException();
	public static void Save(MemoryStream stream, LocoObject obj) => throw new NotImplementedException();
}

[TypeConverter(typeof(ExpandableObjectConverter))]
[LocoStructSize(0x0A)]
[LocoStructType(DatObjectType.Climate)]
internal record DatClimateObject(
	[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
	[property: LocoStructOffset(0x02)] uint8_t FirstSeason,
	[property: LocoStructOffset(0x03), LocoArrayLength(ClimateObjectLoader.Constants.Seasons)] uint8_t[] SeasonLengths,
	[property: LocoStructOffset(0x07)] uint8_t WinterSnowLine,
	[property: LocoStructOffset(0x08)] uint8_t SummerSnowLine,
	[property: LocoStructOffset(0x09), LocoPropertyMaybeUnused] uint8_t pad_09
	)
{

	public bool Validate()
		=> WinterSnowLine <= SummerSnowLine
		&& FirstSeason < 4;
}
