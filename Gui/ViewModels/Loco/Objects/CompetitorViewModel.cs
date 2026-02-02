using Definitions.ObjectModels.Objects.Competitor;

namespace Gui.ViewModels;

public class CompetitorViewModel(CompetitorObject model)
	: BaseViewModel<CompetitorObject>(model)
{
	public NamePrefixFlags AvailableNamePrefixes
	{
		get => Model.AvailableNamePrefixes;
		set => Model.AvailableNamePrefixes = value;
	}

	public PlaystyleFlags AvailablePlayStyles
	{
		get => Model.AvailablePlayStyles;
		set => Model.AvailablePlayStyles = value;
	}

	public EmotionFlags Emotions
	{
		get => Model.Emotions;
		set => Model.Emotions = value;
	}

	public uint8_t Intelligence
	{
		get => Model.Intelligence;
		set => Model.Intelligence = value;
	}

	public uint8_t Aggressiveness
	{
		get => Model.Aggressiveness;
		set => Model.Aggressiveness = value;
	}

	public uint8_t Competitiveness
	{
		get => Model.Competitiveness;
		set => Model.Competitiveness = value;
	}

	public uint8_t var_37
	{
		get => Model.var_37;
		set => Model.var_37 = value;
	}
}
