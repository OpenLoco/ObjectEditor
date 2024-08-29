namespace OpenLoco.ObjectService
{
	// https://learn.microsoft.com/en-us/aspnet/core/performance/rate-limit?view=aspnetcore-8.0
	// https://github.com/dotnet/AspNetCore.Docs.Samples/blob/main/fundamentals/middleware/rate-limit/WebRateLimitAuth/Models/MyRateLimitOptions.cs
	public class ObjectServiceRateLimitOptions
	{
		public const string MyRateLimit = "MyRateLimit";
		//public int PermitLimit { get; set; } = 1;
		//public int Window { get; set; } = 10;
		public int ReplenishmentPeriod { get; set; } = 1;
		public int QueueLimit { get; set; } = 0;
		public int TokenLimit { get; set; } = 20;
		public int TokensReplenishedPerPeriod { get; set; } = 10;
		public bool AutoReplenishment { get; set; } = false;
	}
}
