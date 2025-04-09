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
	}

#if LEGACY_API
	public static class LegacyRoutes
	{
		// used
		public const string ListObjects = "/objects/list";
		public const string GetObjectImages = "/objects/getobjectimages";
		public const string GetObject = "/objects/getobject";

		// unused
		public const string GetDat = "/objects/getdat";
		public const string GetDatFile = "/objects/getdatfile";
		public const string GetObjectFile = "/objects/getobjectfile";
	}
#endif
}
