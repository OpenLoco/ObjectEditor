using OpenLoco.Definitions.Database;

namespace OpenLoco.Definitions.DTO.Mappers
{
	public static class DtoObjectCliffEdgeMapper
	{
		public static DtoObjectCliffEdge ToDto(this TblObjectCliffEdge tblobjectcliffedge)
		{
			return new DtoObjectCliffEdge
			{
				Id = tblobjectcliffedge.Id,
			};
		}

		public static TblObjectCliffEdge ToTblObjectCliffEdgeEntity(this DtoObjectCliffEdge model)
		{
			return new TblObjectCliffEdge
			{
				Id = model.Id,
			};
		}

	}
}

