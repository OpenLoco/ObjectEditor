using OpenLoco.Definitions.Database;

namespace OpenLoco.Definitions.DTO.Mappers
{
	public static class DtoObjectScenarioTextMapper
	{
		public static DtoObjectScenarioText ToDto(this TblObjectScenarioText tblobjectscenariotext)
		{
			return new DtoObjectScenarioText
			{
				Id = tblobjectscenariotext.Id,
			};
		}

		public static TblObjectScenarioText ToTblObjectScenarioTextEntity(this DtoObjectScenarioText model)
		{
			return new TblObjectScenarioText
			{
				Id = model.Id,
			};
		}

	}
}

