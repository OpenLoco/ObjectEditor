namespace OpenLoco.Definitions.Web
{
	public static class OldRoutes
	{

		// GET
		public const string ListObjects = "/objects/list";

		public const string GetDat = "/objects/getdat";
		public const string GetDatFile = "/objects/getdatfile";
		public const string GetObject = "/objects/getobject";
		public const string GetObjectFile = "/objects/getobjectfile";
		public const string GetObjectImages = "/objects/getobjectimages";

		public const string ListObjectPacks = "/objectpacks/list";
		public const string GetObjectPack = "/objectpacks/getpack";

		public const string ListScenarios = "/scenarios/list";
		public const string GetScenario = "/scenarios/getscenario";

		public const string ListSC5FilePacks = "/sc5filepacks/list";
		public const string GetSC5FilePack = "/sc5filepacks/getpack";

		// POST
		public const string UploadDat = "/objects/uploaddat";
		public const string UploadObject = "/objects/uploadobject";

		// PATCH
		public const string UpdateDat = "/objects/updatedat";
		public const string UpdateObject = "/objects/updateobject";
	}

	public static class Routes
	{
		public const string Authors = "/authors";
		public const string Tags = "/tags";
		public const string Licences = "/licences";

		public static string MakePostRoute(string baseRoute)
			=> baseRoute;

		public static string MakeGetRoute(string baseRoute)
			=> $"{baseRoute}/{{id}}";

		public static string MakePutRoute(string baseRoute)
			=> $"{baseRoute}/{{id}}";

		public static string MakeDeleteRoute(string baseRoute)
			=> $"{baseRoute}/{{id}}";

		public static string MakeListRoute(string baseRoute)
			=> baseRoute;
	}
}
