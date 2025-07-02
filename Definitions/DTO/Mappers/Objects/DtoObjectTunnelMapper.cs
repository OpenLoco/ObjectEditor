using OpenLoco.Definitions.Database;

namespace OpenLoco.Definitions.DTO.Mappers
{
	public static class DtoObjectTunnelMapper
	{
		public static DtoObjectTunnel ToDto(this TblObjectTunnel tblobjecttunnel)
		{
			return new DtoObjectTunnel
			{
				Id = tblobjecttunnel.Id,
			};
		}

		public static TblObjectTunnel ToTblObjectTunnelEntity(this DtoObjectTunnel model)
		{
			return new TblObjectTunnel
			{
				Id = model.Id,
			};
		}

	}
}

