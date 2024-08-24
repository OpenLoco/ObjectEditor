using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using ObjectService;
using OpenLoco.ObjectService;
using OpenLoco.Schema.Database;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<LocoDb>(opt => opt.UseSqlite(LocoDb.GetDbPath()));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddHttpLogging(logging => logging.LoggingFields = HttpLoggingFields.All);

//builder.Services.ConfigureHttpJsonOptions(options =>
//{
//	options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
//	options.SerializerOptions.WriteIndented = false;
//});
//builder.Services.AddControllers().AddJsonOptions(options =>
//{
//	options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
//	options.JsonSerializerOptions.WriteIndented = false;
//});

//builder.Services.AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme).AddCertificate();

var tokenPolicy = "token";
var myOptions = new ObjectServiceRateLimitOptions();
builder.Configuration.GetSection(ObjectServiceRateLimitOptions.MyRateLimit).Bind(myOptions);

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

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();
//app.UseAuthentication();
app.UseHttpLogging();
app.UseRateLimiter();

if (app.Environment.IsDevelopment())
{
	_ = app.UseSwagger();
	_ = app.UseSwaggerUI();
}

_ = app.MapGet(Routes.Lookup[RouteName.ListObjects], Server.ListObjects)
	.RequireRateLimiting(tokenPolicy);

_ = app.MapGet(Routes.Lookup[RouteName.GetObject], Server.GetObject)
	.RequireRateLimiting(tokenPolicy);

_ = app.MapPost(Routes.Lookup[RouteName.GetDat], Server.GetDat)
	.RequireRateLimiting(tokenPolicy);

//_ = app.MapPost(Routes.Lookup[RouteName.UploadDat], Server.UploadDat)
//	.RequireRateLimiting(tokenPolicy);

app.Run();
