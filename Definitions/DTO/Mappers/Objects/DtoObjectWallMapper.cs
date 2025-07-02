using OpenLoco.Definitions.Database;

namespace OpenLoco.Definitions.DTO.Mappers
{
	public static class DtoObjectWallMapper
	{
		public static DtoObjectWall ToDto(this TblObjectWall tblobjectwall)
		{
			return new DtoObjectWall
			{
				Height = tblobjectwall.Height,
				Flags1 = tblobjectwall.Flags1,
				Id = tblobjectwall.Id,
			};
		}

		public static TblObjectWall ToTblObjectWallEntity(this DtoObjectWall model)
		{
			return new TblObjectWall
			{
				Height = model.Height,
				Flags1 = model.Flags1,
				Id = model.Id,
			};
		}

	}
}

