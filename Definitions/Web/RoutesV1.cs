namespace Definitions.Web
{
	public static class RoutesV1
	{
		public const string Prefix = "/v1";

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

		public const string ListAuthors = "/authors/list";
		public const string ListLicences = "/licences/list";
		public const string ListTags = "/tags/list";

		// POST
		public const string UploadDat = "/objects/uploaddat";
		public const string UploadObject = "/objects/uploadobject";

		// PATCH
		public const string UpdateDat = "/objects/updatedat";
		public const string UpdateObject = "/objects/updateobject";
	}
}
