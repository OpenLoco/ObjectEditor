using OpenLoco.Definitions.Database;

namespace OpenLoco.Definitions.DTO.Mappers
{
	public static class DtoObjectHillShapesMapper
	{
		public static DtoObjectHillShapes ToDto(this TblObjectHillShapes tblobjecthillshapes)
		{
			return new DtoObjectHillShapes
			{
				HillHeightMapCount = tblobjecthillshapes.HillHeightMapCount,
				MountainHeightMapCount = tblobjecthillshapes.MountainHeightMapCount,
				Flags = tblobjecthillshapes.Flags,
				Id = tblobjecthillshapes.Id,
			};
		}

		public static TblObjectHillShapes ToTblObjectHillShapesEntity(this DtoObjectHillShapes model)
		{
			return new TblObjectHillShapes
			{
				HillHeightMapCount = model.HillHeightMapCount,
				MountainHeightMapCount = model.MountainHeightMapCount,
				Flags = model.Flags,
				Id = model.Id,
			};
		}

	}
}

