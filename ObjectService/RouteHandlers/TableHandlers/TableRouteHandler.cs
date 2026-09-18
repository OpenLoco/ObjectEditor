using Definitions.Database;
using Definitions.DTO;
using Definitions.DTO.Identity;
using Definitions.Web;

namespace ObjectService.RouteHandlers.TableHandlers;

public class AuthorRouteHandler : CrudRouteHandler<DtoAuthorEntry, TblAuthor> { public AuthorRouteHandler() : base(Routes.Authors) { } }
public class TagRouteHandler : CrudRouteHandler<DtoTagEntry, TblTag> { public TagRouteHandler() : base(Routes.Tags) { } }
public class LicenceRouteHandler : CrudRouteHandler<DtoLicenceEntry, TblLicence> { public LicenceRouteHandler() : base(Routes.Licences) { } }
public class UserRouteHandler : CrudRouteHandler<DtoUserEntry, TblUser> { public UserRouteHandler() : base(Routes.Users) { } }
public class RoleRouteHandler : CrudRouteHandler<DtoRoleEntry, TblUserRole> { public RoleRouteHandler() : base(Routes.Roles) { } }
public class ObjectMissingRouteHandler : CrudRouteHandler<DtoObjectMissingEntry, TblObjectMissing> { public ObjectMissingRouteHandler() : base(Routes.Objects + Routes.Missing) { } }
