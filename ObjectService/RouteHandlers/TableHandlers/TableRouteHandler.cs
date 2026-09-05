using Definitions.Database;
using Definitions.DTO;
using Definitions.DTO.Identity;
using Definitions.Web;

namespace ObjectService.RouteHandlers.TableHandlers;

public class AuthorRouteHandler : CrudRouteHandler<DtoAuthorEntry, TblAuthor> { public AuthorRouteHandler() : base(RoutesV2.Authors) { } }
public class TagRouteHandler : CrudRouteHandler<DtoTagEntry, TblTag> { public TagRouteHandler() : base(RoutesV2.Tags) { } }
public class LicenceRouteHandler : CrudRouteHandler<DtoLicenceEntry, TblLicence> { public LicenceRouteHandler() : base(RoutesV2.Licences) { } }
public class UserRouteHandler : CrudRouteHandler<DtoUserEntry, TblUser> { public UserRouteHandler() : base(RoutesV2.Users) { } }
public class RoleRouteHandler : CrudRouteHandler<DtoRoleEntry, TblUserRole> { public RoleRouteHandler() : base(RoutesV2.Roles) { } }
public class ObjectMissingRouteHandler : CrudRouteHandler<DtoObjectMissingEntry, TblObjectMissing> { public ObjectMissingRouteHandler() : base(RoutesV2.Objects + RoutesV2.Missing) { } }
