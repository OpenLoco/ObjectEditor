namespace OpenLoco.Definitions.Web
{
	public static class RoutesV2
	{
		public const string Prefix = "/v2";

		public const string Objects = "/objects";
		public const string ObjectPacks = "/objectpacks";
		public const string Scenarios = "/scenarios";
		public const string SC5FilePacks = "/sc5filepacks";
		public const string Authors = "/authors";
		public const string Tags = "/tags";
		public const string Licences = "/licences";

		// extra Objects routes
		public const string File = "/file";
		public const string Images = "/images";
		public const string Missing = "/missing";

		public const string ResourceRoute = "/{id}";

		// Identity
		public const string Users = "/users";
		public const string Roles = "/roles";
	}
}
