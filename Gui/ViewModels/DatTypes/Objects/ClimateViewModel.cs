using Definitions.ObjectModels.Objects.Climate;
using ReactiveUI.Fody.Helpers;

namespace Gui.ViewModels;

public enum Season
{
	Autumn = 0,
	Winter = 1,
	Spring = 2,
	Summer = 3,
}

public class ClimateViewModel : LocoObjectViewModel<ClimateObject>
{
	[Reactive] public Season FirstSeason { get; set; }
	[Reactive] public uint8_t WinterSnowLine { get; set; }
	[Reactive] public uint8_t SummerSnowLine { get; set; }
	[Reactive] public uint8_t[] SeasonLengths { get; set; }

	public ClimateViewModel(ClimateObject ro)
	{
		FirstSeason = (Season)ro.FirstSeason;
		WinterSnowLine = ro.WinterSnowLine;
		SummerSnowLine = ro.SummerSnowLine;
		SeasonLengths = [ro.SeasonLength1, ro.SeasonLength2, ro.SeasonLength3, ro.SeasonLength4];
	}

	public override ClimateObject GetAsStruct()
		=> new()
		{
			FirstSeason = (uint8_t)FirstSeason,
			WinterSnowLine = WinterSnowLine,
			SummerSnowLine = SummerSnowLine,
			SeasonLength1 = SeasonLengths[0],
			SeasonLength2 = SeasonLengths[1],
			SeasonLength3 = SeasonLengths[2],
			SeasonLength4 = SeasonLengths[3],
		};
}
