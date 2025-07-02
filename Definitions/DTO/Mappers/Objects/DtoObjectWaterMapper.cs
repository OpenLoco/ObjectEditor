using OpenLoco.Definitions.Database;

namespace OpenLoco.Definitions.DTO.Mappers
{
	public static class DtoObjectWaterMapper
	{
		public static DtoObjectWater ToDto(this TblObjectWater tblobjectwater)
		{
			return new DtoObjectWater
			{
				CostIndex = tblobjectwater.CostIndex,
				CostFactor = tblobjectwater.CostFactor,
				Id = tblobjectwater.Id,
			};
		}

		public static TblObjectWater ToTblObjectWaterEntity(this DtoObjectWater model)
		{
			return new TblObjectWater
			{
				CostIndex = model.CostIndex,
				CostFactor = model.CostFactor,
				Id = model.Id,
			};
		}

	}
}

