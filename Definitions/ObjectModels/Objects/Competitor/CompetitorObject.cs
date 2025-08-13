namespace Definitions.ObjectModels.Objects.Competitor;

public class CompetitorObject : ILocoStruct
{
	public CompetitorNamePrefix AvailableNamePrefixes { get; set; } // bitset
	public CompetitorPlaystyle AvailablePlaystyles { get; set; } // bitset
	public uint32_t Emotions { get; set; } // bitset
	public uint8_t Intelligence { get; set; }
	public uint8_t Aggressiveness { get; set; }
	public uint8_t Competitiveness { get; set; }

	public bool Validate() => throw new NotImplementedException();
}
