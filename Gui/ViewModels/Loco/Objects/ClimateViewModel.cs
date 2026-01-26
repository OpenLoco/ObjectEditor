using Definitions.ObjectModels.Objects.Climate;

namespace Gui.ViewModels;

public class ClimateViewModel(ClimateObject model)
	: BaseViewModel<ClimateObject>(model)
{
	public Season FirstSeason
	{
		get => Model.FirstSeason;
		set => Model.FirstSeason = value;
	}

	public uint8_t SeasonLength1
	{
		get => Model.SeasonLength1;
		set => Model.SeasonLength1 = value;
	}

	public uint8_t SeasonLength2
	{
		get => Model.SeasonLength2;
		set => Model.SeasonLength2 = value;
	}

	public uint8_t SeasonLength3
	{
		get => Model.SeasonLength3;
		set => Model.SeasonLength3 = value;
	}

	public uint8_t SeasonLength4
	{
		get => Model.SeasonLength4;
		set => Model.SeasonLength4 = value;
	}

	public uint8_t WinterSnowLine
	{
		get => Model.WinterSnowLine;
		set => Model.WinterSnowLine = value;
	}

	public uint8_t SummerSnowLine
	{
		get => Model.SummerSnowLine;
		set => Model.SummerSnowLine = value;
	}
}
