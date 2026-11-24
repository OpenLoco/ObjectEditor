namespace Definitions.Web;

public static class Routes
{
	// game data
	public const string Objects = "/objects";
	public const string Scenarios = "/scenarios";
	public const string Music = "/music";
	public const string SoundEffects = "/soundeffects";
	public const string Tutorials = "/tutorials";
	public const string Graphics = "/graphics";

	// reference data
	public const string Authors = "/authors";
	public const string Tags = "/tags";
	public const string Licences = "/licences";

	// extra sub-routes
	public const string File = "/file";
	public const string Images = "/images";
	public const string ImageId = "/{imageId:int}";
	public const string ImageMetadata = "/metadata";
	public const string Missing = "/missing";
	public const string Mine = "/mine";

	// packs
	public const string ObjectPacks = "/objectpacks";
	public const string ScenarioPacks = "/scenariopacks";

	// Users (database administration of user records)
	public const string Users = "/users";
	public const string Roles = "/roles";
	public const string Detail = "/detail";
	public const string ClaimsSubRoute = "/claims";
	public const string Lockout = "/lockout";
	public const string EmailConfirmed = "/email-confirmed";
	public const string PasswordReset = "/password-reset";

	// ASP.NET Identity's built-in API plus the self-service account routes for the signed-in user,
	// mounted under Routes.Prefix + Routes.Identity.
	public const string Identity = "/identity";
	public const string IdentityRegister = Identity + "/register";
	public const string IdentityLogin = Identity + "/login";
	public const string IdentityLogout = Identity + "/logout";
	public const string Manage = "/manage";
	public const string Profile = "/profile";
	public const string Account = "/account";
	public const string IdentityManage = Identity + Manage;
	public const string IdentityManageInfo = IdentityManage + "/info";
	public const string IdentityManageProfile = IdentityManage + Profile;
	public const string IdentityManageAccount = IdentityManage + Account;

	// system
	public const string Prefix = "/v2";
	public const string ResourceRoute = "/{id:int}";
	public const string Descriptor = "/descriptor";
	public const string Server = "/server";
	public const string Status = "/status";

}
