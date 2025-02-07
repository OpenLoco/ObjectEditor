namespace OpenLoco.Definitions.Web
{
	public static class OldRoutes
	{
		public const string ListObjects = "/objects/list";
		public const string GetObject = "/objects/getobject";
		public const string UploadObject = "/objects/uploadobject";
		public const string UpdateObject = "/objects/updateobject";
		public const string GetObjectFile = "/objects/getobjectfile";
		public const string GetObjectImages = "/objects/getobjectimages";

		public const string GetDat = "/objects/getdat";
		public const string UploadDat = "/objects/uploaddat";
		public const string GetDatFile = "/objects/getdatfile";
		public const string UpdateDat = "/objects/updatedat";
	}

	public static class Routes
	{
		public const string Objects = "/objects";
		public const string ObjectPacks = "/objectpacks";

		public const string Scenarios = "/scenarios";
		public const string SC5FilePacks = "/sc5filepacks";

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
