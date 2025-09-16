namespace Definitions.ObjectModels.Objects.Climate;

public class ClimateObject : ILocoStruct
{
	public uint8_t FirstSeason { get; set; }
	public uint8_t SeasonLength1 { get; set; }
	public uint8_t SeasonLength2 { get; set; }
	public uint8_t SeasonLength3 { get; set; }
	public uint8_t SeasonLength4 { get; set; }
	public uint8_t WinterSnowLine { get; set; }
	public uint8_t SummerSnowLine { get; set; }

	public bool Validate()
		=> WinterSnowLine <= SummerSnowLine
		&& FirstSeason < 4;
}
