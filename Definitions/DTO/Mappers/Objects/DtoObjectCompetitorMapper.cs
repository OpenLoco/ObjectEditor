using OpenLoco.Definitions.Database;

namespace OpenLoco.Definitions.DTO.Mappers
{
	public static class DtoObjectCompetitorMapper
	{
		public static DtoObjectCompetitor ToDto(this TblObjectCompetitor tblobjectcompetitor)
		{
			return new DtoObjectCompetitor
			{
				AvailableNamePrefixes = tblobjectcompetitor.AvailableNamePrefixes,
				AvailablePlaystyles = tblobjectcompetitor.AvailablePlaystyles,
				Emotions = tblobjectcompetitor.Emotions,
				Intelligence = tblobjectcompetitor.Intelligence,
				Aggressiveness = tblobjectcompetitor.Aggressiveness,
				Competitiveness = tblobjectcompetitor.Competitiveness,
				Id = tblobjectcompetitor.Id,
			};
		}

		public static TblObjectCompetitor ToTblObjectCompetitorEntity(this DtoObjectCompetitor model)
		{
			return new TblObjectCompetitor
			{
				AvailableNamePrefixes = model.AvailableNamePrefixes,
				AvailablePlaystyles = model.AvailablePlaystyles,
				Emotions = model.Emotions,
				Intelligence = model.Intelligence,
				Aggressiveness = model.Aggressiveness,
				Competitiveness = model.Competitiveness,
				Id = model.Id,
			};
		}

	}
}

