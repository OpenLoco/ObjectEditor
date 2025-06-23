namespace OpenLoco.Definitions.Web
{
	public static class Routes
	{
		public const string Objects = "/objects";
		public const string ObjectPacks = "/objectpacks";
		public const string Scenarios = "/scenarios";
		public const string SC5FilePacks = "/sc5filepacks";
		public const string Authors = "/authors";
		public const string Tags = "/tags";
		public const string Licences = "/licences";

		public const string File = "/file";
		public const string Images = "/images";

		public const string ResourceRoute = "/{id}";

		// Identity
		public const string Users = "/users";
		public const string Roles = "/roles";
	}

	public static class LegacyRoutes
	{
		// GET
		public const string ListObjects = "/v1/objects/list";

		public const string GetDat = "/v1/objects/getdat";
		public const string GetDatFile = "/v1/objects/getdatfile";
		public const string GetObject = "/v1/objects/getobject";
		public const string GetObjectFile = "/v1/objects/getobjectfile";
		public const string GetObjectImages = "/v1/objects/getobjectimages";

		public const string ListObjectPacks = "/v1/objectpacks/list";
		public const string GetObjectPack = "/v1/objectpacks/getpack";

		public const string ListScenarios = "/v1/scenarios/list";
		public const string GetScenario = "/v1/scenarios/getscenario";

		public const string ListSC5FilePacks = "/v1/sc5filepacks/list";
		public const string GetSC5FilePack = "/v1/sc5filepacks/getpack";

		public const string ListAuthors = "/v1/authors/list";
		public const string ListLicences = "/v1/licences/list";
		public const string ListTags = "/v1/tags/list";

		// POST
		public const string UploadDat = "/v1/objects/uploaddat";
		public const string UploadObject = "/v1/objects/uploadobject";

		// PATCH
		public const string UpdateDat = "/v1/objects/updatedat";
		public const string UpdateObject = "/v1/objects/updateobject";
	}
}
