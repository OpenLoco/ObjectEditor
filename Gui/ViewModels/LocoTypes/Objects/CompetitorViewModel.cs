using Definitions.ObjectModels.Objects.Competitor;

namespace Gui.ViewModels;

public class CompetitorViewModel : LocoObjectViewModel<CompetitorObject>
{
	public NamePrefixFlags AvailableNamePrefixes { get; set; }
	public PlaystyleFlags AvailablePlaystyles { get; set; }
	public EmotionFlags Emotions { get; set; }
	public uint8_t Intelligence { get; set; }
	public uint8_t Aggressiveness { get; set; }
	public uint8_t Competitiveness { get; set; }
	public uint8_t var_37 { get; set; }

	public CompetitorViewModel(CompetitorObject obj)
	{
		AvailableNamePrefixes = obj.AvailableNamePrefixes;
		AvailablePlaystyles = obj.AvailablePlaystyles;
		Emotions = obj.Emotions;
		Intelligence = obj.Intelligence;
		Aggressiveness = obj.Aggressiveness;
		Competitiveness = obj.Competitiveness;
		var_37 = obj.var_37;
	}

	public override CompetitorObject GetAsModel()
		=> new()
		{
			AvailableNamePrefixes = AvailableNamePrefixes,
			AvailablePlaystyles = AvailablePlaystyles,
			Emotions = Emotions,
			Intelligence = Intelligence,
			Aggressiveness = Aggressiveness,
			Competitiveness = Competitiveness,
			var_37 = var_37,
		};
}
