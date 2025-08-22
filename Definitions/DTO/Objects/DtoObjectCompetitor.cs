using Definitions.Database;
using Definitions.ObjectModels.Objects.Competitor;

namespace Definitions.DTO;

public class DtoObjectCompetitor : IDtoSubObject
{
	public NamePrefixFlags AvailableNamePrefixes { get; set; }
	public PlaystyleFlags AvailablePlaystyles { get; set; }
	public EmotionFlags Emotions { get; set; }
	public uint8_t Intelligence { get; set; }
	public uint8_t Aggressiveness { get; set; }
	public uint8_t Competitiveness { get; set; }
	public UniqueObjectId Id { get; set; }
}
