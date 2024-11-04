using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using OpenLoco.Definitions.Database;
using OpenLoco.Definitions.Web;
using OpenLoco.ObjectService;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var connectionString = builder.Configuration.GetConnectionString("SQLiteConnection");

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<LocoDb>(opt => opt.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddHttpLogging(logging =>
{
	logging.LoggingFields = HttpLoggingFields.ResponsePropertiesAndHeaders
		| HttpLoggingFields.Duration; // this is `All` excluding `ResponseBody`
	logging.CombineLogs = true;
});

var tokenPolicy = "token";
var myOptions = new RateLimitOptions();
builder.Configuration.GetSection(RateLimitOptions.Name).Bind(myOptions);

builder.Services.AddRateLimiter(rlOptions => rlOptions
	.AddTokenBucketLimiter(policyName: tokenPolicy, options =>
	{
		options.TokenLimit = myOptions.TokenLimit;
		options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
		options.QueueLimit = myOptions.QueueLimit;
		options.ReplenishmentPeriod = TimeSpan.FromSeconds(myOptions.ReplenishmentPeriod);
		options.TokensPerPeriod = myOptions.TokensReplenishedPerPeriod;
		options.AutoReplenishment = myOptions.AutoReplenishment;
		rlOptions.OnRejected = (context, cancellationToken) =>
		{
			if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
			{
				context.HttpContext.Response.Headers.RetryAfter = retryAfter.TotalSeconds.ToString();
			}

			context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
			_ = context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", cancellationToken);

			return new ValueTask();
		};
	}));

builder.Services.AddSingleton<Server>();
var serviceSettings = builder.Services.Configure<ServerSettings>(builder.Configuration.GetSection("ObjectService"));

var app = builder.Build();
app.UseHttpLogging();
app.UseRateLimiter();

if (app.Environment.IsDevelopment())
{
	_ = app.UseSwagger();
	_ = app.UseSwaggerUI();
}

var objRoot = builder.Configuration["ObjectService:RootFolder"];
var server = new Server(new ServerSettings(objRoot) { RootFolder = objRoot! });

// GET
_ = app.MapGet(Routes.ListObjects, Server.ListObjects)
	.RequireRateLimiting(tokenPolicy);

_ = app.MapGet(Routes.GetDat, server.GetDat)
	.RequireRateLimiting(tokenPolicy);

_ = app.MapGet(Routes.GetObject, server.GetObject)
	.RequireRateLimiting(tokenPolicy);

_ = app.MapGet(Routes.GetDatFile, server.GetDatFile)
	.RequireRateLimiting(tokenPolicy);

_ = app.MapGet(Routes.GetObjectFile, server.GetObjectFile)
	.RequireRateLimiting(tokenPolicy);

_ = app.MapGet(Routes.ListScenarios, server.ListScenarios)
	.RequireRateLimiting(tokenPolicy);

_ = app.MapGet(Routes.GetScenario, server.GetScenario)
	.RequireRateLimiting(tokenPolicy);

// PATCH
_ = app.MapPatch(Routes.UpdateDat, () => Results.Problem(statusCode: StatusCodes.Status501NotImplemented))
	.RequireRateLimiting(tokenPolicy);

_ = app.MapPatch(Routes.UpdateObject, () => Results.Problem(statusCode: StatusCodes.Status501NotImplemented))
	.RequireRateLimiting(tokenPolicy);

// POST
_ = app.MapPost(Routes.UploadDat, server.UploadDat)
	.RequireRateLimiting(tokenPolicy);

_ = app.MapPost(Routes.UploadObject, /*Server.UploadDat*/ () => Results.Problem(statusCode: StatusCodes.Status501NotImplemented))
	.RequireRateLimiting(tokenPolicy);

app.Run();
