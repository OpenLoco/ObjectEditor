using Asp.Versioning;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using OpenLoco.Definitions.Database;
using OpenLoco.ObjectService;
using System.Text.Json.Serialization;
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
builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddSingleton<Server>();
_ = builder.Services.Configure<ServerSettings>(builder.Configuration.GetSection("ObjectService"));
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

_ = builder.Services.AddApiVersioning(options =>
{
	options.ReportApiVersions = true;
	options.DefaultApiVersion = new ApiVersion(1, 0);
	options.AssumeDefaultVersionWhenUnspecified = true;
	options.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader());
}).AddApiExplorer(options =>
{
	options.GroupNameFormat = "'v'VVV";
	options.SubstituteApiVersionInUrl = true;
});

var app = builder.Build();
app.UseHttpLogging();
app.UseRateLimiter();

var objRoot = builder.Configuration["ObjectService:RootFolder"];
var paletteMapFile = builder.Configuration["ObjectService:PaletteMapFile"];
ArgumentNullException.ThrowIfNull(objRoot);
ArgumentNullException.ThrowIfNull(paletteMapFile);
var server = new Server(new ServerSettings(objRoot, paletteMapFile));

var apiSet = app.NewApiVersionSet().Build();

var groupVersioned = app
	.MapGroup("v{version:apiVersion}")
	.WithApiVersionSet(apiSet)
	.RequireRateLimiting(tokenPolicy)
	.WithTags("Versioned");

server.MapRoutes(groupVersioned);

if (app.Environment.IsDevelopment())
{
	_ = app.UseSwagger();
	_ = app.UseSwaggerUI(
		options =>
		{
			foreach (var description in app.DescribeApiVersions())
			{
				var url = $"/swagger/{description.GroupName}/swagger.json";
				var name = description.GroupName.ToUpperInvariant();
				options.SwaggerEndpoint(url, name);
			}
		});
}

app.Run();
