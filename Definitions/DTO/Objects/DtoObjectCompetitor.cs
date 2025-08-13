using Definitions.Database;
using Definitions.ObjectModels.Objects.Competitor;

namespace Definitions.DTO;

public class DtoObjectCompetitor : IDtoSubObject
{
	public CompetitorNamePrefix AvailableNamePrefixes { get; set; }
	public CompetitorPlaystyle AvailablePlaystyles { get; set; }
	public uint32_t Emotions { get; set; }
	public uint8_t Intelligence { get; set; }
	public uint8_t Aggressiveness { get; set; }
	public uint8_t Competitiveness { get; set; }
	public UniqueObjectId Id { get; set; }
}
