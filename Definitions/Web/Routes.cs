namespace Definitions.Web;

public static class Routes
{
	public const string Prefix = "/v2";

	public const string Objects = "/objects";
	public const string ObjectPacks = "/objectpacks";
	public const string Scenarios = "/scenarios";
	public const string Authors = "/authors";
	public const string Tags = "/tags";
	public const string Licences = "/licences";

	// extra Objects routes
	public const string File = "/file";
	public const string Images = "/images";
	public const string ImageId = "/{imageId:int}";
	public const string Missing = "/missing";
	public const string Mine = "/mine";

	// scenario (SC5) files stored in the database
	public const string SC5Files = "/sc5files";
	public const string SC5FilePacks = "/sc5filepacks";

	public const string ResourceRoute = "/{id:int}";

	// descriptor routes returned by the reference-data handlers
	public const string Descriptor = "/descriptor";

	// server capability/status routes
	public const string Server = "/server";
	public const string Status = "/status";

	// Identity
	public const string Users = "/users";
	public const string Roles = "/roles";
	public const string Me = "/me";
	public const string Detail = "/detail";
	public const string RolesSubRoute = "/roles";
	public const string ClaimsSubRoute = "/claims";
	public const string Lockout = "/lockout";
	public const string EmailConfirmed = "/email-confirmed";
	public const string PasswordReset = "/password-reset";
}
