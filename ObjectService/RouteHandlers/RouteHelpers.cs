namespace ObjectService.RouteHandlers
{
	public static class RouteHelpers
	{
		public static string MakeNicePlural(string name)
			=> $"{name.Replace("RouteHandler", string.Empty)}s";
	}
}
