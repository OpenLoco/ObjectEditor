using Dat.Objects;
using PropertyModels.Extensions;
using ReactiveUI.Fody.Helpers;
using System.ComponentModel;

namespace Gui.ViewModels
{
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
		[Reactive] public BindingList<uint8_t> SeasonLengths { get; set; }

		public ClimateViewModel(ClimateObject ro)
		{
			FirstSeason = (Season)ro.FirstSeason;
			WinterSnowLine = ro.WinterSnowLine;
			SummerSnowLine = ro.SummerSnowLine;
			SeasonLengths = ro.SeasonLengths.ToBindingList();
		}

		public override ClimateObject GetAsStruct(ClimateObject co)
			=> co with
			{
				FirstSeason = (uint8_t)FirstSeason,
				WinterSnowLine = WinterSnowLine,
				SummerSnowLine = SummerSnowLine,
				SeasonLengths = [.. SeasonLengths]
			};
	}
}
