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
	public const string Missing = "/missing";
	public const string Mine = "/mine";

	// packs
	public const string ObjectPacks = "/objectpacks";
	public const string ScenarioPacks = "/scenariopacks";

	// Identity
	public const string Users = "/users";
	public const string Roles = "/roles";
	public const string Me = "/me";
	public const string Detail = "/detail";
	public const string ClaimsSubRoute = "/claims";
	public const string Lockout = "/lockout";
	public const string EmailConfirmed = "/email-confirmed";
	public const string PasswordReset = "/password-reset";

	// system
	public const string Prefix = "/v2";
	public const string ResourceRoute = "/{id:int}";
	public const string Descriptor = "/descriptor";
	public const string Server = "/server";
	public const string Status = "/status";

}
