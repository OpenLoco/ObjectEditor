using Dat.Objects;

namespace Definitions.Database;

public class TblObjectCompetitor : DbSubObject, IConvertibleToTable<TblObjectCompetitor, CompetitorObject>
{
	public CompetitorNamePrefix AvailableNamePrefixes { get; set; } // bitset
	public CompetitorPlaystyle AvailablePlaystyles { get; set; } // bitset
	public uint32_t Emotions { get; set; } // bitset
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
