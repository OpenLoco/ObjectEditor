using Definitions.Database.DataTables;
using Definitions.Database.DataTables.Objects;
using Definitions.DTO.Objects;

namespace Definitions.DTO.Mappers.Objects;

public static class DtoObjectTunnelMapper
{
	public static DtoObjectTunnel ToDto(this TblObjectTunnel tblobjecttunnel) => new()
	{
		Id = tblobjecttunnel.Id,
	};

	public static TblObjectTunnel ToTblObjectTunnelEntity(this DtoObjectTunnel model, TblObject parent) => new()
	{
		Parent = parent,
		Id = model.Id,
	};

}

