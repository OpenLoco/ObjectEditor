using Definitions.Database;
using Definitions.DTO;
using Definitions.DTO.Identity;
using Definitions.Web;
using Microsoft.AspNetCore.Mvc;
using ObjectService.Services;

namespace ObjectService.RouteHandlers.TableHandlers;

public class AuthorRouteHandler : CrudRouteHandler<DtoAuthorEntry, TblAuthor>
{
	public AuthorRouteHandler() : base(Routes.Authors) { }

	public override void MapAdditionalRoutes(IEndpointRouteBuilder baseRoute)
		=> baseRoute.MapGroup(Routes.ResourceRoute).MapGet(Routes.Descriptor, GetDescriptorAsync);

	async Task<IResult> GetDescriptorAsync([FromRoute] UniqueObjectId id, [FromServices] IReferenceDataService svc, CancellationToken ct)
	{
		var dto = await svc.GetAuthorAsync(id, ct);
		return dto != null ? Results.Ok(dto) : Results.NotFound();
	}
}

public class TagRouteHandler : CrudRouteHandler<DtoTagEntry, TblTag>
{
	public TagRouteHandler() : base(Routes.Tags) { }

	public override void MapAdditionalRoutes(IEndpointRouteBuilder baseRoute)
		=> baseRoute.MapGroup(Routes.ResourceRoute).MapGet(Routes.Descriptor, GetDescriptorAsync);

	async Task<IResult> GetDescriptorAsync([FromRoute] UniqueObjectId id, [FromServices] IReferenceDataService svc, CancellationToken ct)
	{
		var dto = await svc.GetTagAsync(id, ct);
		return dto != null ? Results.Ok(dto) : Results.NotFound();
	}
}

public class LicenceRouteHandler : CrudRouteHandler<DtoLicenceEntry, TblLicence>
{
	public LicenceRouteHandler() : base(Routes.Licences) { }

	public override void MapAdditionalRoutes(IEndpointRouteBuilder baseRoute)
		=> baseRoute.MapGroup(Routes.ResourceRoute).MapGet(Routes.Descriptor, GetDescriptorAsync);

	async Task<IResult> GetDescriptorAsync([FromRoute] UniqueObjectId id, [FromServices] IReferenceDataService svc, CancellationToken ct)
	{
		var dto = await svc.GetLicenceAsync(id, ct);
		return dto != null ? Results.Ok(dto) : Results.NotFound();
	}
}

public class RoleRouteHandler : CrudRouteHandler<DtoRoleEntry, TblUserRole> { public RoleRouteHandler() : base(Routes.Roles) { } }
public class ObjectMissingRouteHandler : CrudRouteHandler<DtoObjectMissingEntry, TblObjectMissing> { public ObjectMissingRouteHandler() : base(Routes.Objects + Routes.Missing) { } }
