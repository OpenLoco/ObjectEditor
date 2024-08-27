namespace OpenLoco.Definitions.Web
{
	public static class Routes
	{
		// GET
		public const string ListObjects = "/objects/list";
		public const string GetDat = "/objects/getdat";
		public const string GetObject = "/objects/getobject";
		public const string GetDatFile = "/objects/getdatfile";
		public const string GetObjectFile = "/objects/getobjectfile";

		// POST
		public const string UploadDat = "/objects/uploaddat";
		public const string UploadObject = "/objects/uploadobject";

		// PATCH
		public const string UpdateDat = "/objects/updatedat";
		public const string UpdateObject = "/objects/updateobject";
	}
}