using Definitions.ObjectModels.Objects.Competitor;

namespace Definitions.Database;

public class TblObjectCompetitor : DbSubObject, IConvertibleToTable<TblObjectCompetitor, CompetitorObject>
{
	public NamePrefixFlags AvailableNamePrefixes { get; set; } // bitset
	public PlaystyleFlags AvailablePlaystyles { get; set; } // bitset
	public EmotionFlags Emotions { get; set; } // bitset
	public uint8_t Intelligence { get; set; }
	public uint8_t Aggressiveness { get; set; }
	public uint8_t Competitiveness { get; set; }

	//public uint8_t var_37 { get; set; }

	public static TblObjectCompetitor FromObject(TblObject tbl, CompetitorObject obj)
		=> new()
		{
			Parent = tbl,
			AvailableNamePrefixes = obj.AvailableNamePrefixes,
			AvailablePlaystyles = obj.AvailablePlaystyles,
			Emotions = obj.Emotions,
			Intelligence = obj.Intelligence,
			Aggressiveness = obj.Aggressiveness,
			Competitiveness = obj.Competitiveness,
		};
}
