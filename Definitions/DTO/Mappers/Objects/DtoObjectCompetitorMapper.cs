using Definitions.Database;

namespace Definitions.DTO.Mappers;

public static class DtoObjectCompetitorMapper
{
	public static DtoObjectCompetitor ToDto(this TblObjectCompetitor tblobjectcompetitor) => new()
	{
		AvailableNamePrefixes = tblobjectcompetitor.AvailableNamePrefixes,
		AvailablePlaystyles = tblobjectcompetitor.AvailablePlaystyles,
		Emotions = tblobjectcompetitor.Emotions,
		Intelligence = tblobjectcompetitor.Intelligence,
		Aggressiveness = tblobjectcompetitor.Aggressiveness,
		Competitiveness = tblobjectcompetitor.Competitiveness,
		Id = tblobjectcompetitor.Id,
	};

	public static TblObjectCompetitor ToTblObjectCompetitorEntity(this DtoObjectCompetitor model, TblObject parent) => new()
	{
		Parent = parent,
		AvailableNamePrefixes = model.AvailableNamePrefixes,
		AvailablePlaystyles = model.AvailablePlaystyles,
		Emotions = model.Emotions,
		Intelligence = model.Intelligence,
		Aggressiveness = model.Aggressiveness,
		Competitiveness = model.Competitiveness,
		Id = model.Id,
	};

}

