using OpenLoco.Dat.Objects;
using PropertyModels.Extensions;
using ReactiveUI.Fody.Helpers;
using System.ComponentModel;

namespace OpenLoco.Gui.ViewModels
{
	public class ClimateViewModel : LocoObjectViewModel<ClimateObject>
	{
		[Reactive] public uint8_t FirstSeason { get; set; }
		[Reactive] public uint8_t WinterSnowLine { get; set; }
		[Reactive] public uint8_t SummerSnowLine { get; set; }
		[Reactive] public uint8_t var_09 { get; set; }
		[Reactive] public BindingList<uint8_t> SeasonLengths { get; set; }

		public ClimateViewModel(ClimateObject ro)
		{
			FirstSeason = ro.FirstSeason;
			WinterSnowLine = ro.WinterSnowLine;
			SummerSnowLine = ro.SummerSnowLine;
			var_09 = ro.var_09;
			SeasonLengths = ro.SeasonLengths.ToBindingList();
		}

		public override ClimateObject GetAsStruct(ClimateObject co)
			=> co with
			{
				FirstSeason = FirstSeason,
				WinterSnowLine = WinterSnowLine,
				SummerSnowLine = SummerSnowLine,
				var_09 = var_09,
				SeasonLengths = [.. SeasonLengths]
			};
	}
}
