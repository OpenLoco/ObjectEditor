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
		public const string CreateUser = "/users/create";


		// PATCH
		public const string UpdateDat = "/objects/updatedat";
		public const string UpdateObject = "/objects/updateobject";

		public const string AddRole = "/users/{userid}/roles/add/{tagid}";
		public const string RemoveRole = "/users/{userid}/roles/remove/{tagid}";

		public const string AddAuthor = "/users/{userid}/author/add/{authorid}";
		public const string RemoveAuthor = "/users/{userid}/author/remove/{authorid}";

		public const string AddModpack
	}
}
